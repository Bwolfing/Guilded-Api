﻿using Guilded.Areas.Admin.Controllers;
using Guilded.Areas.Admin.Data.DAL;
using Guilded.Tests.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Guilded.Tests.Areas.Admin.RolesControllerTests
{
    public class RolesControllerTest : ControllerTest<RolesController>
    {
        protected Mock<IRolesDataContext> MockAdminDataContext;
        protected Mock<ILoggerFactory> MockLoggerFactory;

        protected override RolesController SetUpController()
        {
            MockAdminDataContext = new Mock<IRolesDataContext>();
            MockLoggerFactory = new Mock<ILoggerFactory>();

            return new RolesController(
                MockAdminDataContext.Object,
                MockLoggerFactory.Object
            );
        }
    }
}