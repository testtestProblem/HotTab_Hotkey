using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

namespace CollectDataAP
{
    class Connect2UWP
    {
        private AppServiceConnection connection = null;

        public async void InitializeAppServiceConnection()
        {
            connection = new AppServiceConnection();
            connection.AppServiceName = "SampleInteropService";
            connection.PackageFamilyName = Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;
            connection.ServiceClosed += Connection_ServiceClosed;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            if (status != AppServiceConnectionStatus.Success)
            {
                // something went wrong ...
                Console.WriteLine(status.ToString());
                Console.ReadLine();
                //this.IsEnabled = false;
            }
        }

        public async void SendData2UWP(uint data)
        {
            // ask the UWP to calculate d1 + d2
            ValueSet request = new ValueSet();
            request.Add("deviceStateAll", (uint)data);
            //request.Add("D2", (double)2);
            await connection.SendMessageAsync(request);
            //AppServiceResponse response = await connection.SendMessageAsync(request);
            //string result = (string)response.Message["RESULT"];
        }

        /// <summary>
        /// Handles the event when the desktop process receives a request from the UWP app
        /// </summary>
        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {/*
            DeviceState deviceState = new DeviceState();

            // retrive the reg key name from the ValueSet in the request
            uint? key = args.Request.Message["deviceConfig"] as uint?;

            uint state = deviceState.GetDeviceStatePower();
            try
            {
                foreach (uint device in Enum.GetValues(typeof(DeviceState.DeviceStatePower)))
                {
                    if ((key & device) == device) 
                    { 
                        state = state ^ (uint)key;
                        deviceState.SetDeviceStatePower(state);
                    }
                }
            }
            catch (Exception e)
            {
                //TODO - not verify
                var dialog = new MessageDialog(e.Message);
                await dialog.ShowAsync();
            }
            // compose the response as ValueSet
            ValueSet response = new ValueSet();

            state = deviceState.GetDeviceStatePower();
            response.Add("res_deviceConfig", state);

            // send the response back to the UWP
            await args.Request.SendResponseAsync(response);


            */
            /*
            if (key == "wifi")
            {
                // compose the response as ValueSet
                ValueSet response = new ValueSet();
                response.Add("res_wifi", "enable");

                // send the response back to the UWP
                await args.Request.SendResponseAsync(response);
            }
            else
            {
                ValueSet response = new ValueSet();
                //response.Add("ERROR", "INVALID REQUEST");
                await args.Request.SendResponseAsync(response);
            }*/
        }

        /// <summary>
        /// Handles the event when the app service connection is closed
        /// </summary>
        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            Console.WriteLine("UWP Disconnect! Please restart APP!");
        }
    }
}
