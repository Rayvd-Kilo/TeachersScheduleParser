using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using TeachersScheduleParser.MVVM.Objects;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.MVVM.View;

public partial class ClientListView : IDisposable
{
    private readonly IDataContainerModel<ClientData[]> _clientDataModel;
    private readonly IAsyncReactiveValue<ClientData> _reactiveValue;

    public ClientListView()
    {
        InitializeComponent();

        _clientDataModel = App.AppHost!.Services.GetRequiredService<IDataContainerModel<ClientData[]>>();
        _reactiveValue = App.AppHost!.Services.GetRequiredService<IAsyncReactiveValue<ClientData>>();

        _reactiveValue.ValueChangedAsync += UpdateListAsync;

        UpdateListAsync(new ClientData());
    }
    
    public void Dispose()
    {
        _reactiveValue.ValueChangedAsync -= UpdateListAsync;
    }
    
    private Task UpdateListAsync(ClientData data)
    {
        var clientData = _clientDataModel.GetData();

        var dataList = new List<ClientDataObject>();

        foreach (var client in clientData!)
        {
            dataList.Add(new ClientDataObject()
            {
                ClientName = client.ProfileName,
                SubscriptionType = client.SubscriptionType,
                RequirePersonType = client.RequirePersonType
            }); 
        }

        if (Dispatcher.CheckAccess())
        {
            ClientsDataTable.ItemsSource = dataList;
            
            return Task.CompletedTask;
        }
        
        Dispatcher.Invoke(() =>
        {
            ClientsDataTable.ItemsSource = dataList;
        });

        return Task.CompletedTask;
    }
}