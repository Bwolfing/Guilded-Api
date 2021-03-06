﻿using Guilded.Areas.Forums.DAL;
using Guilded.Areas.Forums.ViewModels;
using Guilded.Extensions;
using Guilded.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Guilded.Areas.Forums.Controllers
{
    [Route("[area]/[controller]")]
    public class RepliesController : ForumsController
    {
        public RepliesController(IForumsDataContext dataContext, ILoggerFactory loggerFactory) : base(dataContext, loggerFactory)
        {
        }

        [Authorize(RoleClaimValues.ForumsWriterClaim)]
        [HttpPost("new/{threadId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostNewReplyToThread(CreateReplyViewModel reply)
        {
            var thread = await DataContext.GetThreadByIdAsync(reply.ThreadId);

            if (thread.IsNotFound())
            {
                ModelState.AddModelError("", "That thread does not exist.");
            }
            else if (thread.IsLocked)
            {
                ModelState.AddModelError("", "You cannot reply to that thread since it is locked.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var createdReply = await DataContext.CreateReplyAsync(reply.ToReply(User.UserId()));

                    return PartialView("DisplayTemplates/ReplyViewModel", new ReplyViewModel(createdReply));
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e);

                    ModelState.AddModelError("", "An error occurred while creating the reply.");
                }
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("CreateReplyViewModel", reply);
        }

        [HttpGet("{replyId}")]
        public async Task<IActionResult> UpdateReply(int replyId)
        {
            var reply = await DataContext.GetReplyByIdAsync(replyId);
            if (reply.IsNotFound())
            {
                return NotFound("That reply does not exist.");
            }

            if (reply.Thread.IsLocked)
            {
                return BadRequest("You cannot edit a post in a locked thread.");
            }

            if (reply.AuthorId != User.UserId())
            {
                return StatusCode(HttpStatusCode.Unauthorized, "You are not the author of this post.");
            }

            return Content(reply.Content);
        }

        [Authorize(RoleClaimValues.ForumsWriterClaim)]
        [HttpPost("{replyId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReply(UpdateReplyViewModel reply)
        {
            var dbReply = await DataContext.GetReplyByIdAsync(reply.ReplyId);
            if (dbReply.IsNotFound())
            {
                return NotFound("That reply does not exist.");
            }

            if (dbReply.Thread.IsLocked)
            {
                return BadRequest("You cannot edit a post in a locked thread.");
            }

            if (dbReply.AuthorId != User.UserId())
            {
                return StatusCode(HttpStatusCode.Unauthorized, "You are not the author of this post.");
            }

            try
            {
                dbReply = await DataContext.UpdateReplyContentByIdAsync(reply.ReplyId, reply.Content);
                return PartialView(new ReplyViewModel(dbReply));
            }
            catch (Exception e)
            {
                Logger.LogError($"An error occurred attempting to update the content of reply {reply.ReplyId}.", e);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [Authorize(RoleClaimValues.ForumsWriterClaim)]
        [HttpDelete("{threadId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReply(int replyId)
        {
            var reply = await DataContext.GetReplyByIdAsync(replyId);

            if (reply == null || reply.IsDeleted || reply.Thread.IsDeleted || !reply.Thread.Forum.IsActive)
            {
                return NotFound();
            }

            if (reply.Thread.IsLocked)
            {
                return BadRequest("The thread is locked, therefore you cannot delete the reply.");
            }

            if (reply.AuthorId != User.UserId())
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "You are not the author of this post.");
            }

            try
            {
                await DataContext.DeleteReplyAsync(reply);

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred with your request.");
        }
    }
}
