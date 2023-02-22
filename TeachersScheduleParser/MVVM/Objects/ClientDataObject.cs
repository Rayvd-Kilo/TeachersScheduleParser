using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.MVVM.Objects;

public class ClientDataObject
{
    public string ClientName { get; set; }
    
    public SubscriptionType SubscriptionType { get; set; }
    
    public UpdateType UpdateType { get; set; }

    public PersonType RequirePersonType { get; set; }
}