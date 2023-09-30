using FluentAssertions;
using FluentAssertions.Extensions;
using NetworkUltility.DNS;
using NetworkUltility.Ping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Xunit;

namespace NetworkUltility.Tests.PingTests
{
    public class NetworkServiceTests
    {
        private readonly NetworkService _pingService;
        private readonly IDNS _dNS;
        public NetworkServiceTests()
        {
            //Dependences
            _dNS = A.Fake<IDNS>();
            //SUT
            _pingService = new NetworkService(_dNS);
        }
        [Fact]
        public void NetworkService_SendPing_ReturnString()
        {
            //Arrange - variables, classes, mocks
            A.CallTo(()=> _dNS.SendDNS()).Returns(true);
            //Act
            var result = _pingService.SendPing();
            //Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Be("Success: Ping Sent!");
            result.Should().Contain("Success", Exactly.Once());
            
        }
        [Theory]
        [InlineData(1,1,2)]
        [InlineData(2,2,4)]
        public void NetworkService_PingTimeOut_ReturnInt(int a, int b, int expected)
        {
            //Arrange
            
            //Act
            var result = _pingService.PingTimeOut(a,b);
            //Assert
            result.Should().Be(expected);
            result.Should().BeGreaterThanOrEqualTo(2);
            result.Should().NotBeInRange(-10000, 0);
        }
        [Fact]
        public void NetworkService_lastPingDate_ReturnDate()
        {
            //Arrange

            //Act
            var result = _pingService.LastPingDate();
            //Assert
            result.Should().BeAfter(1.January(2010));
            result.Should().BeBefore(1.January(2024));
        }
        [Fact]
        public void NetworkService_GetPingOptions_ReturnsObject()
        {
            //Arrange
            var expected = new PingOptions() 
            { 
                DontFragment = true,
                Ttl = 1,
            };
            //Act
            var result = _pingService.GetPingOptions();
            //Assert WARNING: Be careful
            result.Should().BeOfType<PingOptions>();
            result.Should().BeEquivalentTo(expected);
            result.Ttl.Should().Be(1);
        }
        [Fact]
        public void NetworkService_MostRecentPings_ReturnObject()
        {
            //Arrange
            var expected = new PingOptions()
            {
                DontFragment = true,
                Ttl = 1,
            };
            //Act
            var result = _pingService.MostRecentPings();
            //Assert
            //result.Should().BeOfType<IEnumerable<PingOptions>>();
            result.Should().ContainEquivalentOf(expected);
            result.Should().Contain(x => x.DontFragment == true);
        }
    }
}
