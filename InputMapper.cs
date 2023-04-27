using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SteamDeckController;
using WindowsInput;

static class InputMapper {
  const double padToMouseFactor = 1 / 150.0;
  const double padToScrollFactor = 1 / 500.0;


  private static byte ScaleInt16ToByte(short value) {
    // First, we'll scale the Int16 range to a float range between 0 and 1
    float scaledFloat = (value - short.MinValue) / (float)(short.MaxValue - short.MinValue);

    // Next, we'll convert the float value to a byte by scaling it to the byte range
    byte scaledByte = (byte)(scaledFloat * byte.MaxValue);

    return scaledByte;
  }

  public static void MapInput(
  SteamDeckController.InputState inputState,
  SteamDeckController.InputState prevInputState,
  InputSimulator inputSimulator,
  Nefarius.ViGEm.Client.Targets.IXbox360Controller xboxController) {
    // simulate mouse clicks
    {
      if (inputState.RightPadPress && !prevInputState.RightPadPress) {
        inputSimulator.Mouse.RightButtonDown();
      }
      if (!inputState.RightPadPress && prevInputState.RightPadPress) {
        inputSimulator.Mouse.RightButtonUp();
      }
      if (inputState.LeftPadPress && !prevInputState.LeftPadPress) {
        inputSimulator.Mouse.LeftButtonDown();
      }
      if (!inputState.LeftPadPress && prevInputState.LeftPadPress) {
        inputSimulator.Mouse.LeftButtonUp();
      }
    }

    // simulate mouse movement
    {
      if (inputState.RightPadX != 0 && prevInputState.RightPadY != 0 ||
          inputState.RightPadY != 0 && prevInputState.RightPadY != 0) {
        var xDelta = inputState.RightPadX - prevInputState.RightPadX;
        var yDelta = -(inputState.RightPadY - prevInputState.RightPadY);
        inputSimulator.Mouse.MoveMouseBy(
          (int)(xDelta * padToMouseFactor),
          (int)(yDelta * padToMouseFactor)
        );
      }
    }

    // simulate mouse scroll
    {
      if (inputState.LeftPadX != 0 && prevInputState.LeftPadX != 0) {
        var xDelta = inputState.LeftPadX - prevInputState.LeftPadX;
        inputSimulator.Mouse.HorizontalScroll((int)(xDelta * padToScrollFactor));
      }
      if (inputState.LeftPadY != 0 && prevInputState.LeftPadY != 0) {
        var yDelta = inputState.LeftPadY - prevInputState.LeftPadY;
        inputSimulator.Mouse.VerticalScroll((int)(yDelta * padToScrollFactor));
      }
    }

    // simulate Xbox controller input
    {
      xboxController.SetButtonState(Xbox360Button.A, inputState.ButtonA);
      xboxController.SetButtonState(Xbox360Button.B, inputState.ButtonB);
      xboxController.SetButtonState(Xbox360Button.X, inputState.ButtonX);
      xboxController.SetButtonState(Xbox360Button.Y, inputState.ButtonY);

      xboxController.SetButtonState(Xbox360Button.LeftShoulder, inputState.ButtonL1);
      xboxController.SetButtonState(Xbox360Button.RightShoulder, inputState.ButtonR1);

      xboxController.SetButtonState(Xbox360Button.Back, inputState.ButtonMenu);
      xboxController.SetButtonState(Xbox360Button.Start, inputState.ButtonOptions);
      xboxController.SetButtonState(Xbox360Button.Guide, inputState.ButtonSteam);

      xboxController.SetButtonState(Xbox360Button.LeftThumb, inputState.ButtonLeftStickPress);
      xboxController.SetButtonState(Xbox360Button.RightThumb, inputState.ButtonRightStickPress);

      xboxController.SetButtonState(Xbox360Button.Up, inputState.ButtonDpadUp);
      xboxController.SetButtonState(Xbox360Button.Right, inputState.ButtonDpadRight);
      xboxController.SetButtonState(Xbox360Button.Down, inputState.ButtonDpadDown);
      xboxController.SetButtonState(Xbox360Button.Left, inputState.ButtonDpadLeft);

      xboxController.SetSliderValue(Xbox360Slider.LeftTrigger, ScaleInt16ToByte(inputState.AxisLeftTrigger));
      xboxController.SetSliderValue(Xbox360Slider.RightTrigger, ScaleInt16ToByte(inputState.AxisRightTrigger));

      xboxController.SetAxisValue(Xbox360Axis.LeftThumbX, inputState.AxisLeftStickX);
      xboxController.SetAxisValue(Xbox360Axis.LeftThumbY, inputState.AxisLeftStickY);
      xboxController.SetAxisValue(Xbox360Axis.RightThumbX, inputState.AxisRightStickX);
      xboxController.SetAxisValue(Xbox360Axis.RightThumbY, inputState.AxisRightStickY);
    }
  }
}
