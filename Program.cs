using static System.Runtime.InteropServices.Marshal;
using static SteamDeckController;
using WindowsInput;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using static HidApi;

internal class Program {
  private static void Main(string[] args) {
    var steamDeckControllerHandle = CreateSteamDeckControllerHandle();

    // disable steam deck controller lizard mode
    DisableLizardMode(steamDeckControllerHandle);

    // initialize mouse and keyboard input simulator
    InputSimulator inputSimulator = new InputSimulator();

    // initialize simulated xbox controller
    var vigEmclient = new ViGEmClient();
    var xboxController = vigEmclient.CreateXbox360Controller();
    xboxController.Connect();

    // read and map input buffer from steam deck controller
    var inputBuffer = new byte[64];
    var prevInputState = new SteamDeckController.InputState();

    while (true) {
      int bytesRead = hid_read_timeout(steamDeckControllerHandle, inputBuffer, 64, 100);
      if (bytesRead > 0) {
        try {
          var inputState = new SteamDeckController.InputState(inputBuffer);
          InputMapper.MapInput(inputState, prevInputState, inputSimulator, xboxController);
          prevInputState = inputState;
        } catch (Exception e) {
          Console.WriteLine(e.Message);
          Console.WriteLine(e.StackTrace);
        }
      }
    }
  }
}
