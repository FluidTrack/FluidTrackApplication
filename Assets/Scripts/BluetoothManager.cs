using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class BluetoothManager : MonoBehaviour {
	public static BluetoothManager GetInstance() {
		return GameObject.Find("[SYSTEM] Total Manager").GetComponent<BluetoothManager>();
	}

	public string DeviceName = "TouchW32_9E";
	public string MacAddress = "";
	public string ServiceUUID = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
	public string SendUUID = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
	public string ReceiveUUID = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
	public ProtocolHandler Protocol;

	public enum States {
		None,
		Scan,
		Connect,
		Subscribe,
		Unsubscribe,
		Disconnect,
		Communication,
	}

	internal bool _workingFoundDevice = true;
	internal bool _connected = false;
	internal float _timeout = 0f;
	internal States _state = States.None;
	internal bool _foundID = false;


	void Reset() {
		_workingFoundDevice = false;    // used to guard against trying to connect to a second device while still connecting to the first
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_foundID = false;
		MacAddress = null;
	}

	public void SetState(States newState, float timeout) {
		_state = newState;
		_timeout = timeout;
	}

	void StartProcess() {

		Reset();
		BluetoothLEHardwareInterface.Initialize(true, false, () => {

			SetState(States.Scan, 0.1f);

		}, (error) => {
			//AlertHandler.GetInstance().Pop_Error("BT Init실패");
			BluetoothLEHardwareInterface.Log("Error: " + error);
            if(Welcome5Handler.GetInstance() != null &&
                Welcome5Handler.GetInstance().gameObject.activeSelf)
			    Welcome5Handler.GetInstance().BlindControl(false);
		});
	}


	public void OnConnectStart(string deviceName, string macAddress, string serviceUUID, string sendUUID, string receiveUUID) {
        if (Welcome5Handler.GetInstance() != null &&
                Welcome5Handler.GetInstance().gameObject.activeSelf)
                 Welcome5Handler.GetInstance().BlindControl(true);
        DeviceName = deviceName;
		MacAddress = macAddress;
		ServiceUUID = serviceUUID;
		SendUUID = sendUUID;
		ReceiveUUID = receiveUUID;
		StartProcess();
	}

	// Update is called once per frame
	void Update() {
		if (_timeout > 0f) {
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f) {
				_timeout = 0f;

				switch (_state) {
					case States.None:
					break;

					case States.Scan:

					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) => {

						// we only want to look at devices that have the name we are looking for
						// this is the best way to filter out devices
						if (name.Contains(DeviceName)) {
							_workingFoundDevice = true;

							// it is always a good idea to stop scanning while you connect to a device
							// and get things set up
							BluetoothLEHardwareInterface.StopScan();

							// add it to the list and set to connect to it
							MacAddress = address;

							SetState(States.Connect, 2f);
							//MainPanelHandler.GetInstance().Connect();
							_workingFoundDevice = false;
						}

					}, null, false, false);
					break;

					case States.Connect:
					// set these flags
					_foundID = false;
                    

					// note that the first parameter is the address, not the name. I have not fixed this because
					// of backwards compatiblity.
					// also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
					// the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
					// large enough that it will be finished enumerating before you try to subscribe or do any other operations.
					BluetoothLEHardwareInterface.ConnectToPeripheral(MacAddress, null, null, (address, serviceUUID, characteristicUUID) => {
						if (IsEqual(serviceUUID, ServiceUUID)) {
							// if we have found the characteristic that we are waiting for
							// set the state. make sure there is enough timeout that if the
							// device is still enumerating other characteristics it finishes
							// before we try to subscribe
							if (IsEqual(characteristicUUID, ReceiveUUID)) {

								_connected = true;
                                StartCoroutine(ConnectAndQuery());
                                SetState(States.Subscribe, 2f);
								TotalManager.instance.BlindControl(true);
                                if (Welcome5Handler.GetInstance() != null &&
                                    Welcome5Handler.GetInstance().gameObject.activeSelf)
                                    Welcome5Handler.GetInstance().BlindControl(false);
							}
						}
					}, (disconnectedAddress) => {
                        if (Welcome5Handler.GetInstance() != null &&
                             Welcome5Handler.GetInstance().gameObject.activeSelf)
                                Welcome5Handler.GetInstance().BlindControl(false);
                        TotalManager.instance.BlindControl(false);
						BluetoothLEHardwareInterface.Log("Device disconnected: " + disconnectedAddress);
						AlertHandler.GetInstance().Pop_Discon(DeviceName);
					});
					break;

					case States.Subscribe:




					//========================================

					BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(MacAddress, ServiceUUID, ReceiveUUID, (notifyAddress, notifyCharacteristic) => {
                        
						_state = States.None;

						// read the initial state of the button
						BluetoothLEHardwareInterface.ReadCharacteristic(MacAddress, ServiceUUID, ReceiveUUID, (characteristic, bytes) => {
							//Protocol.ParsingBytes(bytes);
						});

					}, (address, characteristicUUID, bytes) => {
						if (_state != States.None) {
							// some devices do not properly send the notification state change which calls
							// the lambda just above this one so in those cases we don't have a great way to
							// set the state other than waiting until we actually got some data back.
							// The esp32 sends the notification above, but if yuor device doesn't you would have
							// to send data like pressing the button on the esp32 as the sketch for this demo
							// would then send data to trigger this.

							_state = States.None;
						}

						// we received some data from the device
						Protocol.ParsingBytes(bytes);
					});

					//========================================
					break;

					case States.Unsubscribe:
					BluetoothLEHardwareInterface.UnSubscribeCharacteristic(MacAddress, ServiceUUID, ReceiveUUID, null);
					SetState(States.Disconnect, 4f);
					break;

					case States.Disconnect:
					if (_connected) {
						BluetoothLEHardwareInterface.DisconnectPeripheral(MacAddress, (address) => {
							BluetoothLEHardwareInterface.DeInitialize(() => {
								TotalManager.instance.Disconnect();
								_connected = false;
                                SoundHandler.Instance.Play_SFX(SoundHandler.SFX.DISCONNECT);
                                _state = States.None;
							});
						});
					} else {
						BluetoothLEHardwareInterface.DeInitialize(() => {
							TotalManager.instance.Disconnect();
                            SoundHandler.Instance.Play_SFX(SoundHandler.SFX.DISCONNECT);
                            _state = States.None;
						});
					}
					break;
				}
			}
		}
	}

	string FullUUID(string uuid) {
		return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
	}

	bool IsEqual(string uuid1, string uuid2) {
		if (uuid1.Length == 4)
			uuid1 = FullUUID(uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID(uuid2);

		return ( uuid1.ToUpper().Equals(uuid2.ToUpper()) );
	}

	public void SetCurrentTime() {
		var data = ProtocolHandler.SetTimerToCurrent();
		SendBytes(data);
	}

	public void SetTime(int year, int month, int day, int hour, int min, int sec) {
		var data = ProtocolHandler.SetTimer(year, month, day, hour, min, sec);
		SendBytes(data);
	}

	public void QueryBattery() {
		var data = ProtocolHandler.QueryBattery();
		SendBytes(data);
	}

	void SendString(string value) {
		var data = Encoding.UTF8.GetBytes(value);
		// notice that the 6th parameter is false. this is because the TouchW32 doesn't support withResponse writing to its characteristic.
		// some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
		// the device
		BluetoothLEHardwareInterface.WriteCharacteristic(MacAddress, ServiceUUID, SendUUID, data, data.Length, false, (characteristicUUID) => {

			BluetoothLEHardwareInterface.Log("Write Succeeded");
		});
	}

	void SendByte(byte value) {
		byte[] data = new byte[] { value };
		// notice that the 6th parameter is false. this is because the TouchW32 doesn't support withResponse writing to its characteristic.
		// some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
		// the device
		BluetoothLEHardwareInterface.WriteCharacteristic(MacAddress, ServiceUUID, SendUUID, data, data.Length, false, (characteristicUUID) => {

			BluetoothLEHardwareInterface.Log("Write Succeeded");
		});
	}

	void SendBytes(byte[] data) {
		if (!_connected) {
			AlertHandler.GetInstance().Pop_Alert("모아밴드와 연결을 먼저 해 주세요");
			return;
		}
		// notice that the 6th parameter is false. this is because the TouchW32 doesn't support withResponse writing to its characteristic.
		// some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
		// the device
		BluetoothLEHardwareInterface.WriteCharacteristic(MacAddress, ServiceUUID, SendUUID, data, data.Length, false, (characteristicUUID) => {

			BluetoothLEHardwareInterface.Log("Write Succeeded");
		});
	}

    public void QueryHistory() {
        var data = ProtocolHandler.GetHistory();
        SendBytes(data);
    }

    public void SetRedLED() {
        var data = ProtocolHandler.GetRedLEDOn();
        SendBytes(data);
    }

    public void SetGreenLED() {
        var data = ProtocolHandler.GetGreenLEDOn();
        SendBytes(data);
    }

    public void SetBlueLED() {
        var data = ProtocolHandler.GetBlueLEDOn();
        SendBytes(data);
    }

    public void SetVibrate() {
        var data = ProtocolHandler.GetVibrateOn();
        SendBytes(data);
    }

    IEnumerator ConnectAndQuery() {
        yield return new WaitForSeconds(3f);
        if (TotalManager.instance.isRegisterMode)
            Welcome5Handler.Instance.RegisterUI.SetActive(true);
        SoundHandler.Instance.Play_SFX(SoundHandler.SFX.CONNECT);
        ProtocolHandler.SetTimerToCurrent();
        QueryHistory();
        QueryBattery();
        if (MoabandStatusHandler.Instance != null && MoabandStatusHandler.Instance.gameObject.activeSelf)
            MoabandStatusHandler.Instance.ConnectTimeRefresh();
    }
}
