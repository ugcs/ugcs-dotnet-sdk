using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows.Threading;
#endif
using MapControl;
using UGCS.Example.Models;
using Services;
using System.Windows;
using UGCS.Example.Properties;
using UGCS.Sdk.Protocol.Encoding;
using System.Threading.Tasks;
using Services.Helpers;
using Services.SdkServices;
using System.Collections.Generic;
using System.Timers;
using UGCS.Example.Enums;
using Services.Commands;

namespace UGCS.Example.ViewModels
{
    public partial class MainViewModel : Caliburn.Micro.PropertyChangedBase
    {


        private async Task updateVehicle()
        {
            var task = Task.Factory.StartNew(() =>
            {
                _vehicleService.SaveVehicleFields(ClientVehicle.Vehicle, new string[] { "profile" });
            });
            await task;
        }

        public async Task AutoModeVehicle()
        {
            await Task.Run(() => 
            {
                _commandService.SendSingleCommand(ClientVehicle.Vehicle, _vehicleCommand.GetCommand(VehicleCommand.CommandName.AutoMode));
            }).ContinueWith((result) =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(result.Exception.Message);
                }
            });
        }

        public async Task ManualModeVehicle()
        {
            await Task.Run(() =>
            {
                _commandService.SendSingleCommand(ClientVehicle.Vehicle, _vehicleCommand.GetCommand(VehicleCommand.CommandName.ManualMode));
            }).ContinueWith((result) =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(result.Exception.Message);
                }
            });
        }

        public async Task ArmVehicle()
        {
            await Task.Run(() =>
            {
                _commandService.SendSingleCommand(ClientVehicle.Vehicle, _vehicleCommand.GetCommand(VehicleCommand.CommandName.Arm));
            }).ContinueWith((result) =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(result.Exception.Message);
                }
            });
        }

        public async Task DisarmVehicle()
        {
            await Task.Run(() =>
            {
                _commandService.SendSingleCommand(ClientVehicle.Vehicle, _vehicleCommand.GetCommand(VehicleCommand.CommandName.Disarm));
            }).ContinueWith((result) =>
            {
                if (result.Exception != null)
                {
                    MessageBox.Show(result.Exception.Message);
                }
            }); 
        }

public async Task SetTakeOffPoint()
{
    await Task.Run(() =>
    {
        ClientVehicle.Vehicle.AltitudeOrigin = ClientVehicle.Telemetry.Elevation;
        ClientVehicle.Vehicle.AltitudeOriginSpecified = true;
        _vehicleService.SaveVehicleFields(ClientVehicle.Vehicle, new string[] { "altitudeOrigin" });
    }).ContinueWith((result) => 
    {
        if (result.Exception != null)
        {
            MessageBox.Show(result.Exception.Message);
        }
    });
}

        public async Task CaluclateRoute()
        {
            if (Route == null)
            {
                MessageBox.Show("Route not created");
                return;
            }
            await Task.Run(() =>
            {
                ProcessedRoute = _routeService.CalculateRoute(Route);
            }).ContinueWith((result) =>
            {
                if (result.Exception != null && result.Exception.InnerExceptions.Count > 0)
                {
                    MessageBox.Show(String.Join("\n", result.Exception.InnerExceptions.Select(x => x.Message).ToList<String>()));
                }
            });
        }

        public async Task UploadRouteVehicle()
        {
            if (Route == null)
            {
                MessageBox.Show("Route not created");
                return;
            }
            if (ProcessedRoute == null)
            {
                MessageBox.Show("Route not calculated");
            } 
            else if (ClientVehicle.Vehicle.AltitudeOriginSpecified == false)
            {
                MessageBox.Show("Takeoff altitude not specified");
            }
            else
            {
                await Task.Run(() =>
                {
                    _routeService.UploadRoute(ProcessedRoute, ClientVehicle.Vehicle);
                }).ContinueWith((result) =>
                {
                    if (result.Exception != null)
                    {
                        MessageBox.Show(result.Exception.Message);
                    }
                });
            }
        }
        
    }
}
