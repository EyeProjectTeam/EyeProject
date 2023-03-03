using EyeProtect.Application;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EyeProtect.Manage.Timers
{
    public class TimerService : IHostedService
    {
        private System.Timers.Timer _timer;
        private readonly IMemberService _memberService;

        public TimerService(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new System.Timers.Timer()
            {
                Enabled = true,
                Interval = 1000 * 60 * 60 * 24 * 1, //一天执行一次
            };
            _timer.Elapsed += ExpireAccount;
            _timer.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void ExpireAccount(object source, ElapsedEventArgs e)
        {
            await _memberService.ExpireAccount();
        }
    }
}
