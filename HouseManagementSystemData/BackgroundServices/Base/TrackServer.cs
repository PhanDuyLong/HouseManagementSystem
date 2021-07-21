using HMS.Data.Services;
using HMS.Data.ViewModels.Contract.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HMSAPI
{
    /// <summary>
    /// TrackerServer
    /// </summary>
    public class TrackServer : BackgroundService
    {
        private static readonly int IDLE_MINUTES = 5;
        private static readonly int IDLE_DAYS = 1;
        private readonly ILogger<TrackServer> _logger;
        private readonly List<ContractDetailViewModel> contractNotficationList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="services"></param>
        public TrackServer(ILogger<TrackServer> logger, IServiceProvider services)
        {
            _logger = logger;
            Services = services;
        }
        /// <summary>
        /// Services
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Run TrackServer
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = Services.CreateScope())
                {
                    var contractService = scope.ServiceProvider.GetRequiredService<IContractService>();
                    var billService = scope.ServiceProvider.GetRequiredService<IBillService>();
                    await contractService.ScanContracts();
                    //await billService.ScanBill();
                }
                _logger.LogInformation("Track Server running at {time}", DateTimeOffset.Now);
                //await Task.Delay(IDLE_MINUTES * 60 * 1000, stoppingToken);
                await Task.Delay(IDLE_DAYS * 24 * 60 * 60 * 1000, stoppingToken);
            }
        }
    }
}
