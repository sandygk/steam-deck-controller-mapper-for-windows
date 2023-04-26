using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.Marshal;

static class SteamDeckController {

  public static UInt16 vendorId = 10462;
  public static UInt16 productId = 4613;
  public static UInt16 inputBufferLen = 64;
    

  [StructLayout(LayoutKind.Sequential)]
  public struct HidInputState {
    public byte ptype;          //0x00
    public byte _a1;            //0x01 
    public byte _a2;            //0x02 
    public byte _a3;            //0x03
    public UInt32 seq;          //0x04 
    public UInt16 buttons0;     //0x09 
    public byte buttons1;       //0x0A
    public byte buttons2;       //0x0C
    public byte buttons3;       //0x0D
    public byte buttons4;       //0x0E
    public byte buttons5;       //0x0E
    public Int16 lpad_x;        //0x10
    public Int16 lpad_y;        //0x12
    public Int16 rpad_x;        //0x13
    public Int16 rpad_y;        //0x16
    public Int16 accel_x;       //0x18
    public Int16 accel_y;       //0x1A
    public Int16 accel_z;       //0x1C
    public Int16 gpitch;        //0x1E
    public Int16 gyaw;          //0x20
    public Int16 groll;         //0x22
    public Int16 q1;            //0x24
    public Int16 q2;            //0x26
    public Int16 q3;            //0x28
    public Int16 q4;            //0x2A
    public Int16 ltrig;         //0x2C
    public Int16 rtrig;         //0x2E
    public Int16 lthumb_x;      //0x30
    public Int16 lthumb_y;      //0x32
    public Int16 rthumb_x;      //0x34
    public Int16 rthumb_y;      //0x36
    public Int16 lpad_pressure; //0x38
    public Int16 rpad_pressure; //0x3A

    public static bool isPressed(UInt16 inputButtonsGroup, UInt16 valueToCheck) {
      return (inputButtonsGroup & valueToCheck) == valueToCheck;
    }
  }

  public struct InputState {
    public bool buttonA;
    public bool buttonB;
    public bool buttonX;
    public bool buttonY;

    public bool buttonDpadDown;
    public bool buttonDpadUp;
    public bool buttonDpadLeft;
    public bool buttonDpadRight;

    public bool buttonMenu;
    public bool buttonSteam;
    public bool buttonOptions;
    public bool buttonQuickAccess;

    public bool buttonL1;
    public bool buttonL2;
    public bool buttonL4;
    public bool buttonL5;

    public bool buttonR1;
    public bool buttonR2;
    public bool buttonR4;
    public bool buttonR5;

    public bool leftPadTouch;
    public bool leftPadPress;
    public bool rightPadTouch;
    public bool rightPadPress;

    public bool leftStickTouch;
    public bool leftStickPress;
    public bool rightStickTouch;
    public bool rightStickPress;

    public Int16 leftStickX;
    public Int16 leftStickY;
    public Int16 rightStickX;
    public Int16 rightStickY;

    public Int16 leftPadX;
    public Int16 leftPadY;
    public Int16 rightPadX;
    public Int16 rightPadY;

    public Int16 leftPadPressure;
    public Int16 rightPadPressure;

    public Int16 leftTrigger;
    public Int16 rightTrigger;

    public Int16 gyroAccelX;
    public Int16 gyroAccelY;
    public Int16 gyroAccelZ;

    public Int16 gyroYaw;
    public Int16 gyroRoll;
    public Int16 gyroPitch;

    public Int16 q1;
    public Int16 q2;
    public Int16 q3;
    public Int16 q4;

    public InputState(byte[] buffer) {
      // convert byte[] buffer to HidInputState struct
      var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
      HidInputState hidInput;
      try {
        hidInput = (HidInputState)PtrToStructure(handle.AddrOfPinnedObject(), typeof(HidInputState));
      }
      finally {
        handle.Free();
      }

      buttonA = HidInputState.isPressed(hidInput.buttons0, 0b0000000010000000);
      buttonB = HidInputState.isPressed(hidInput.buttons0, 0b0000000000100000);
      buttonX = HidInputState.isPressed(hidInput.buttons0, 0b0000000001000000);
      buttonY = HidInputState.isPressed(hidInput.buttons0, 0b0000000000010000);

      buttonDpadDown = HidInputState.isPressed(hidInput.buttons0, 0b0000100000000000);
      buttonDpadUp = HidInputState.isPressed(hidInput.buttons0, 0b0000000100000000);
      buttonDpadLeft = HidInputState.isPressed(hidInput.buttons0, 0b0000010000000000);
      buttonDpadRight = HidInputState.isPressed(hidInput.buttons0, 0b0000001000000000);

      buttonMenu = HidInputState.isPressed(hidInput.buttons0, 0b0001000000000000);
      buttonSteam = HidInputState.isPressed(hidInput.buttons0, 0b0010000000000000);
      buttonOptions = HidInputState.isPressed(hidInput.buttons0, 0b0100000000000000);
      buttonQuickAccess = HidInputState.isPressed(hidInput.buttons5, 0b00000100);

      buttonL1 = HidInputState.isPressed(hidInput.buttons0, 0b0000000000001000);
      buttonL2 = HidInputState.isPressed(hidInput.buttons0, 0b0000000000000010);
      buttonL4 = HidInputState.isPressed(hidInput.buttons4, 0b00000010);
      buttonL5 = HidInputState.isPressed(hidInput.buttons0, 0b1000000000000000);

      buttonR1 = HidInputState.isPressed(hidInput.buttons0, 0b0000000000000100);
      buttonR2 = HidInputState.isPressed(hidInput.buttons0, 0b0000000000000001);
      buttonR4 = HidInputState.isPressed(hidInput.buttons4, 0b00000100);
      buttonR5 = HidInputState.isPressed(hidInput.buttons1, 0b00000001);

      leftPadTouch = HidInputState.isPressed(hidInput.buttons1, 0b00001000);
      leftPadPress = HidInputState.isPressed(hidInput.buttons1, 0b00000010);
      rightPadTouch = HidInputState.isPressed(hidInput.buttons1, 0b00010000);
      rightPadPress = HidInputState.isPressed(hidInput.buttons1, 0b00000100);

      leftStickTouch = HidInputState.isPressed(hidInput.buttons4, 0b01000000);
      leftStickPress = HidInputState.isPressed(hidInput.buttons1, 0b01000000);
      rightStickTouch = HidInputState.isPressed(hidInput.buttons4, 0b10000000);
      rightStickPress = HidInputState.isPressed(hidInput.buttons2, 0b00000100);

      leftStickX = hidInput.lthumb_x;
      leftStickY = hidInput.lthumb_y;
      rightStickX = hidInput.rthumb_x;
      rightStickY = hidInput.rthumb_y;

      leftPadX = hidInput.lpad_x;
      leftPadY = hidInput.lpad_y;
      rightPadX = hidInput.rpad_x;
      rightPadY = hidInput.rpad_y;

      leftPadPressure = hidInput.lpad_pressure;
      rightPadPressure = hidInput.rpad_pressure;

      leftTrigger = hidInput.ltrig;
      rightTrigger = hidInput.rtrig;

      gyroAccelX = hidInput.accel_x;
      gyroAccelY = hidInput.accel_y;
      gyroAccelZ = hidInput.accel_z;

      gyroYaw = hidInput.gyaw;
      gyroRoll = hidInput.groll;
      gyroPitch = hidInput.gpitch;
      q1 = hidInput.q1;
      q2 = hidInput.q2;
      q3 = hidInput.q3;
      q4 = hidInput.q4;
    }
  }
}
