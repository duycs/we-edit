using Application.Models;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class RouteTests : TestBase
    {
        private IRouteService _routeService;

        [OneTimeSetUp]
        public void Init()
        {
            _routeService = _serviceProvider.GetService<IRouteService>();
        }

        [Test]
        [TestCase()]
        public void Add_3_Routes_Success()
        {
            var createRouteVM1 = new CreateRouteVM()
            {
                FromOperationId = 1,
                ToOperationId = 2,
            };

            var createRouteVM2 = new CreateRouteVM()
            {
                FromOperationId = 2,
                ToOperationId = 1,
            };

            var createRouteVM3 = new CreateRouteVM()
            {
                FromOperationId = 1,
                ToOperationId = 1,
            };

            var route1 = _routeService.CreateRoute(createRouteVM1);
            var route2 = _routeService.CreateRoute(createRouteVM2);
            var route3 = _routeService.CreateRoute(createRouteVM3);

            Assert.NotNull(route1);
            Assert.NotNull(route2);
            Assert.NotNull(route3);
        }
    }
}
