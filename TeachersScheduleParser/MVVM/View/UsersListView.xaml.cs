using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

using TeachersScheduleParser.MVVM.Objects;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.MVVM.View;

public partial class UsersListView : UserControl
{
    private readonly IDataContainerModel<ClientData[]> _clientDataModel;
    private readonly IAsyncReactiveValue<ClientData> _reactiveValue;

    public UsersListView(
        IDataContainerModel<ClientData[]> clientDataModel,
        IAsyncReactiveValue<ClientData> reactiveValue)
    {
        _clientDataModel = clientDataModel;
        _reactiveValue = reactiveValue;

        InitializeComponent();

        reactiveValue.ValueChangedAsync += UpdateListAsync;

        UpdateListAsync(new ClientData());
    }

    public UsersListView()
    {
        InitializeComponent();
    }

    private Task UpdateListAsync(ClientData data)
    {
        var clientData = _clientDataModel.GetData();

        var dataList = new List<ClientDataObject>();

        foreach (var client in clientData)
        {
           dataList.Add(new ClientDataObject()
           {
               ClientName = client.ProfileName,
               SubscriptionType = client.SubscriptionType,
               RequirePersonType = client.RequirePersonType
           }); 
        }

        //ClientsDataList.DataContext = dataList;
        
        return Task.CompletedTask;
    }
}