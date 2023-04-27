using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.Marshal;
using static HidApi;
using static SteamDeckController.Button;
using static SteamDeckController.Axis;
using System.Runtime.CompilerServices;

static class SteamDeckController {

  public static UInt16 vendorId = 10462;
  public static UInt16 productId = 4613;
  public static UInt16 inputBufferLen = 64;
  private static byte[] disableMouseEmulationBuffer = new byte[] { 0x87, 0x03, 0x08, 0x07 };
  private static byte[] disableButtonEmulationBuffer = new byte[] { 0x81, 0x00 };
  private static byte[] featureReportRequest = new byte[inputBufferLen + 1];
  private static byte[] featureReportResponse = new byte[inputBufferLen + 1];

  [StructLayout(LayoutKind.Sequential)]
  public struct InputState {
    private byte ptype;         
    private byte _a1;            
    private byte _a2;            
    private byte _a3;           
    private UInt32 seq;          
    private UInt16 buttons0;     
    private byte buttons1;      
    private byte buttons2;      
    private byte buttons3;      
    private byte buttons4;      
    private byte buttons5;      
    private Int16 lpad_x;       
    private Int16 lpad_y;       
    private Int16 rpad_x;       
    private Int16 rpad_y;       
    private Int16 accel_x;      
    private Int16 accel_y;      
    private Int16 accel_z;      
    private Int16 gpitch;       
    private Int16 gyaw;         
    private Int16 groll;        
    private Int16 q1;           
    private Int16 q2;           
    private Int16 q3;           
    private Int16 q4;           
    private Int16 ltrig;        
    private Int16 rtrig;        
    private Int16 lthumb_x;     
    private Int16 lthumb_y;     
    private Int16 rthumb_x;     
    private Int16 rthumb_y;     
    private Int16 lpad_pressure;
    private Int16 rpad_pressure;

    public bool ButtonA => GetButton(Button.A);
    public bool ButtonB => GetButton(Button.B);
    public bool ButtonX => GetButton(Button.X);
    public bool ButtonY => GetButton(Button.Y);
    public bool ButtonDpadDown => GetButton(Button.DpadDown);
    public bool ButtonDpadUp => GetButton(Button.DpadUp);
    public bool ButtonDpadLeft => GetButton(Button.DpadLeft);
    public bool ButtonDpadRight => GetButton(Button.DpadRight);
    public bool ButtonMenu => GetButton(Button.Menu);
    public bool ButtonSteam => GetButton(Button.Steam);
    public bool ButtonOptions => GetButton(Button.Options);
    public bool QuickAccess => GetButton(Button.QuickAccess);
    public bool ButtonL1 => GetButton(Button.L1);
    public bool L2 => GetButton(Button.L2);
    public bool L4 => GetButton(Button.L4);
    public bool L5 => GetButton(Button.L5);
    public bool ButtonR1 => GetButton(Button.R1);
    public bool R2 => GetButton(Button.R2);
    public bool R4 => GetButton(Button.R4);
    public bool R5 => GetButton(Button.R5);
    public bool LeftPadTouch => GetButton(Button.LeftPadTouch);
    public bool LeftPadPress => GetButton(Button.LeftPadPress);
    public bool RightPadTouch => GetButton(Button.RightPadTouch);
    public bool RightPadPress => GetButton(Button.RightPadPress);
    public bool LeftStickTouch => GetButton(Button.LeftStickTouch);
    public bool ButtonLeftStickPress => GetButton(Button.LeftStickPress);
    public bool RightStickTouch => GetButton(Button.RightStickTouch);
    public bool ButtonRightStickPress => GetButton(Button.RightStickPress);

    public short AxisLeftStickX => GetAxis(Axis.LeftStickX);
    public short AxisLeftStickY => GetAxis(Axis.LeftStickY);
    public short AxisRightStickX => GetAxis(Axis.RightStickX);
    public short AxisRightStickY => GetAxis(Axis.RightStickY);
    public short LeftPadX => GetAxis(Axis.LeftPadX);
    public short LeftPadY => GetAxis(Axis.LeftPadY);
    public short RightPadX => GetAxis(Axis.RightPadX);
    public short RightPadY => GetAxis(Axis.RightPadY);
    public short LeftPadPressure => GetAxis(Axis.LeftPadPressure);
    public short RightPadPressure => GetAxis(Axis.RightPadPressure);
    public short AxisLeftTrigger => GetAxis(Axis.LeftTrigger);
    public short AxisRightTrigger => GetAxis(Axis.RightTrigger);
    public short GyroAccelX => GetAxis(Axis.GyroAccelX);
    public short GyroAccelY => GetAxis(Axis.GyroAccelY);
    public short GyroAccelZ => GetAxis(Axis.GyroAccelZ);
    public short GyroYaw => GetAxis(Axis.GyroYaw);
    public short GyroRoll => GetAxis(Axis.GyroRoll);
    public short GyroPitch => GetAxis(Axis.GyroPitch);
    public short Q1 => GetAxis(Axis.Q1);
    public short Q2 => GetAxis(Axis.Q2);
    public short Q3 => GetAxis(Axis.Q3);
    public short Q4 => GetAxis(Axis.Q4);

    public InputState(byte[] buffer) {
      var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
      InputState hidInput;
      try {
        this = (InputState)PtrToStructure(handle.AddrOfPinnedObject(), typeof(InputState));
      }
      finally {
        handle.Free();
      }
    }

    public bool GetButton(SteamDeckController.Button button) {
      switch (button) {
        case Button.A: return IsSet(buttons0, 0b0000000010000000);
        case Button.B: return IsSet(buttons0, 0b0000000000100000);
        case Button.X: return IsSet(buttons0, 0b0000000001000000);
        case Button.Y: return IsSet(buttons0, 0b0000000000010000);
        case Button.DpadDown: return IsSet(buttons0, 0b0000100000000000);
        case Button.DpadUp: return IsSet(buttons0, 0b0000000100000000);
        case Button.DpadLeft: return IsSet(buttons0, 0b0000010000000000);
        case Button.DpadRight: return IsSet(buttons0, 0b0000001000000000);
        case Button.Menu: return IsSet(buttons0, 0b0001000000000000);
        case Button.Steam: return IsSet(buttons0, 0b0010000000000000);
        case Button.Options: return IsSet(buttons0, 0b0100000000000000);
        case Button.QuickAccess: return IsSet(buttons5, 0b00000100);
        case Button.L1: return IsSet(buttons0, 0b0000000000001000);
        case Button.L2: return IsSet(buttons0, 0b0000000000000010);
        case Button.L4: return IsSet(buttons4, 0b00000010);
        case Button.L5: return IsSet(buttons0, 0b1000000000000000);
        case Button.R1: return IsSet(buttons0, 0b0000000000000100);
        case Button.R2: return IsSet(buttons0, 0b0000000000000001);
        case Button.R4: return IsSet(buttons4, 0b00000100);
        case Button.R5: return IsSet(buttons1, 0b00000001);

        case Button.LeftPadTouch: return IsSet(buttons1, 0b00001000);
        case Button.LeftPadPress: return IsSet(buttons1, 0b00000010);
        case Button.RightPadTouch: return IsSet(buttons1, 0b00010000);
        case Button.RightPadPress: return IsSet(buttons1, 0b00000100);

        case Button.LeftStickTouch: return IsSet(buttons4, 0b01000000);
        case Button.LeftStickPress: return IsSet(buttons1, 0b01000000);
        case Button.RightStickTouch: return IsSet(buttons4, 0b10000000);
        case Button.RightStickPress: return IsSet(buttons2, 0b00000100);
        default:
          throw new NotImplementedException("Unsupported Steam Deck Controller Button passed as argument");
      }
    }

    public Int16 GetAxis(SteamDeckController.Axis axis) {
      switch (axis) {
        case Axis.LeftStickX: return lthumb_x;
        case Axis.LeftStickY: return lthumb_y;
        case Axis.RightStickX: return rthumb_x;
        case Axis.RightStickY: return rthumb_y;

        case Axis.LeftPadX: return lpad_x;
        case Axis.LeftPadY: return lpad_y;
        case Axis.RightPadX: return rpad_x;
        case Axis.RightPadY: return rpad_y;

        case Axis.LeftPadPressure: return lpad_pressure;
        case Axis.RightPadPressure: return rpad_pressure;
        case Axis.LeftTrigger: return ltrig;
        case Axis.RightTrigger: return rtrig;
        case Axis.GyroAccelX: return accel_x;
        case Axis.GyroAccelY: return accel_y;
        case Axis.GyroAccelZ: return accel_z;
        case Axis.GyroYaw: return gyaw;
        case Axis.GyroRoll: return groll;
        case Axis.GyroPitch: return gpitch;
        case Axis.Q1: return q1;
        case Axis.Q2: return q2;
        case Axis.Q3: return q3;
        case Axis.Q4: return q4;
        default:
          throw new NotImplementedException("Unsupported Steam Deck Controller Axis passed as argument");
      }
    }

    private bool IsSet(UInt16 buttonGroup, UInt16 buttonFlag) {
      return (buttonGroup & buttonFlag) == buttonFlag;
    }
  }

  public enum Button {
    A, B, X, Y,
    DpadDown, DpadUp, DpadLeft, DpadRight,
    Menu, Steam, Options, QuickAccess,
    L1, L2, L4, L5,
    R1, R2, R4, R5,
    LeftPadTouch, LeftPadPress, RightPadTouch, RightPadPress,
    LeftStickTouch, LeftStickPress, RightStickTouch, RightStickPress
  }

  public enum Axis {
    LeftStickX, LeftStickY, RightStickX, RightStickY,
    LeftPadX, LeftPadY, RightPadX, RightPadY,
    LeftPadPressure, RightPadPressure,
    LeftTrigger, RightTrigger,
    GyroAccelX, GyroAccelY, GyroAccelZ,
    GyroYaw, GyroRoll, GyroPitch,
    Q1, Q2, Q3, Q4
  }

  private static void RequestFeatureReport(IntPtr deviceHandle, byte[] requestBody) {
    if (requestBody.Length > SteamDeckController.inputBufferLen)
      throw new ArgumentException("requestBody length is greater than input inputBuffer length.");
    
    Array.Clear(featureReportRequest, 0, featureReportRequest.Length);
    Array.Clear(featureReportResponse, 0, featureReportResponse.Length);

    Array.Copy(requestBody, 0, featureReportRequest, 1, requestBody.Length);

    int err = hid_send_feature_report(deviceHandle, featureReportRequest, (uint)(SteamDeckController.inputBufferLen + 1));
    if (err < 0) {
      throw new Exception($"Could not send report to hid device. Error: {err}");
    }
    err = hid_get_feature_report(deviceHandle, featureReportResponse, (uint)(SteamDeckController.inputBufferLen + 1));
    if (err < 0) {
      throw new Exception($"Could not get report from hid device. Error: {err}");
    }
  }

  public static void DisableLizardModeForAWhile(IntPtr steamDeckControllerHandle) {
    RequestFeatureReport(steamDeckControllerHandle, disableMouseEmulationBuffer);
    RequestFeatureReport(steamDeckControllerHandle, disableButtonEmulationBuffer);
  }

  public static void DisableLizardMode(IntPtr steamDeckControllerHandle) {
    DisableLizardModeForAWhile(steamDeckControllerHandle);
    System.Timers.Timer disableLizardModeTimer = new System.Timers.Timer(1000);
    disableLizardModeTimer.Elapsed += (s, e) => DisableLizardModeForAWhile(steamDeckControllerHandle);
    disableLizardModeTimer.Start();
  }

  public static IntPtr CreateSteamDeckControllerHandle() {
    var deviceHandle = IntPtr.Zero;
    IntPtr deviceInfoPtr = hid_enumerate(SteamDeckController.vendorId, SteamDeckController.productId);
    while (deviceInfoPtr != IntPtr.Zero) {
      hid_device_info deviceInfo = PtrToStructure<hid_device_info>(deviceInfoPtr);
      string path = PtrToStringAnsi(deviceInfo.path)!;
      deviceHandle = hid_open_path(path);
      if (deviceHandle != IntPtr.Zero) break;

      deviceInfoPtr = deviceInfo.next;
    }

    if (deviceHandle == IntPtr.Zero) {
      Console.WriteLine("Steam Deck Controller not found!");
    }
    return deviceHandle;
  }
}

