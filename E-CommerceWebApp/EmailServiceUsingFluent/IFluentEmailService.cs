namespace E_CommerceWebApp.EmailServiceUsingFluent
{
    public interface IFluentEmailService
    {
        void SendEmailForOrder(string reciepentEmail, string reciepentName, string subject, string description);
    }
}
