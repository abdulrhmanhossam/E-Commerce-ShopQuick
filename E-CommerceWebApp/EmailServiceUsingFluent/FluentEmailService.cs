using FluentEmail.Core;

namespace E_CommerceWebApp.EmailServiceUsingFluent
{
    public class FluentEmailService : IFluentEmailService
    {
        private readonly IServiceProvider _serviceProvider;
        public FluentEmailService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async void SendEmailForOrder(string reciepentEmail, string reciepentName, string subject, string description)
        {
            using (var info = _serviceProvider.CreateScope())
            {
                var mailer = info.ServiceProvider.GetRequiredService<IFluentEmail>();
                var email = mailer
                    .To(reciepentEmail, reciepentName)
                    .Subject(subject)
                    .Body(description);
                await email.SendAsync();
            }
        }

    }
}
