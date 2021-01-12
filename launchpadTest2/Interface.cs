using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midi;
#pragma warning disable IDE1006 // 命名样式
namespace LP
{
    public class Interface
    {
        private Pitch[,] notes = new Pitch[8, 8] {
            { Pitch.A5, Pitch.ASharp5, Pitch.B5, Pitch.C6, Pitch.CSharp6, Pitch.D6, Pitch.DSharp6, Pitch.E6 },
            { Pitch.B4, Pitch.C5, Pitch.CSharp5, Pitch.D5, Pitch.DSharp5, Pitch.E5, Pitch.F5, Pitch.FSharp5 },
            { Pitch.CSharp4, Pitch.D4, Pitch.DSharp4, Pitch.E4, Pitch.F4, Pitch.FSharp4, Pitch.G4, Pitch.GSharp4 },
            { Pitch.DSharp3, Pitch.E3, Pitch.F3, Pitch.FSharp3, Pitch.G3, Pitch.GSharp3, Pitch.A3, Pitch.ASharp3 },
            { Pitch.F2, Pitch.FSharp2, Pitch.G2, Pitch.GSharp2, Pitch.A2, Pitch.ASharp2, Pitch.B2, Pitch.C3 },
            { Pitch.G1, Pitch.GSharp1, Pitch.A1, Pitch.ASharp1, Pitch.B1, Pitch.C2, Pitch.CSharp2, Pitch.D2 },
            { Pitch.A0, Pitch.ASharp0, Pitch.B0, Pitch.C1, Pitch.CSharp1, Pitch.D1, Pitch.DSharp1, Pitch.E1 },
            { Pitch.BNeg1, Pitch.C0, Pitch.CSharp0, Pitch.D0, Pitch.DSharp0, Pitch.E0, Pitch.F0, Pitch.FSharp0 }
        };

        private Pitch[] rightLEDnotes = new Pitch[] {
            Pitch.F6, Pitch.G5, Pitch.A4, Pitch.B3, Pitch.CSharp3, Pitch.DSharp2, Pitch.F1, Pitch.G0
        };
        public enum ControlButton
        {
            UP,DOWN,LEFT,RIGHT,SESSION,USER1,USER2,MIXER,F0,F1,F2,F3,F4,F5,F6,F7,NONE
        }

        public InputDevice targetInput;
        public OutputDevice targetOutput;

        public delegate void LaunchpadKeyEventHandler(object source, LaunchpadKeyEventArgs e);

        public delegate void LaunchpadCCKeyEventHandler(object source, LaunchpadCCKeyEventArgs e);

        public event LaunchpadKeyEventHandler OnLaunchpadKeyPressed;
        public event LaunchpadCCKeyEventHandler OnLaunchpadCCKeyPressed;

        public class LaunchpadCCKeyEventArgs : EventArgs
        {
            private ControlButton val;
            private bool press;
            public LaunchpadCCKeyEventArgs(int _val,int _vol)
            {
                switch (_val)
                {
                    case 104:
                        val = ControlButton.UP;
                        break;
                    case 105:
                        val = ControlButton.DOWN;
                        break;
                    case 106:
                        val = ControlButton.LEFT;
                        break;
                    case 107:
                        val = ControlButton.RIGHT;
                        break;
                    case 108:
                        val = ControlButton.SESSION;
                        break;
                    case 109:
                        val = ControlButton.USER1;
                        break;
                    case 110:
                        val = ControlButton.USER2;
                        break;
                    case 111:
                        val = ControlButton.MIXER;
                        break;
                    case 89:
                        val = ControlButton.F0;
                        break;
                    case 79:
                        val = ControlButton.F1;
                        break;
                    case 69:
                        val = ControlButton.F2;
                        break;
                    case 59:
                        val = ControlButton.F3;
                        break;
                    case 49:
                        val = ControlButton.F4;
                        break;
                    case 39:
                        val = ControlButton.F5;
                        break;
                    case 29:
                        val = ControlButton.F6;
                        break;
                    case 19:
                        val = ControlButton.F7;
                        break;
                    default:
                        val = ControlButton.NONE;
                        break;
                }
                if (_vol == 127) press = true;
                else press = false;
            }
            public ControlButton GetVal()
            {
                return val;
            }
            public bool isPressing()
            {
                return press;
            }
        }

        public class LaunchpadKeyEventArgs : EventArgs
        {
            private int x;
            private int y;
            private bool press;
            private Midi.NoteOnMessage msg;
            public LaunchpadKeyEventArgs(int _pX, int _pY,int _vol)
            {
                x = _pX;
                y = _pY;
                msg = null;
                if (_vol == 127) press = true;
                else press = false;
            }
            public LaunchpadKeyEventArgs(Midi.NoteOnMessage _msg,int _pX, int _pY,int _vol)
            {
                msg = _msg;
                x = _pX;
                y = _pY;
                if (_vol == 127) press = true;
                else press = false;
            }
            public int GetX()
            {
                return x;
            }
            public int GetY()
            {
                return y;
            }
            public Midi.NoteOnMessage getMSG()
            {
                return msg;
            }
            public bool isPressing()
            {
                return press;
            }
        }


        private void midiPress(Midi.NoteOnMessage msg)
        {
            if (OnLaunchpadKeyPressed != null && !rightLEDnotes.Contains(msg.Pitch))
            {
                OnLaunchpadKeyPressed(this, new LaunchpadKeyEventArgs(msg,midiNoteToLed(msg.Pitch)[0], midiNoteToLed(msg.Pitch)[1],msg.Velocity));
            }
            else if (OnLaunchpadCCKeyPressed != null && rightLEDnotes.Contains(msg.Pitch))
            {
                OnLaunchpadCCKeyPressed(this, new LaunchpadCCKeyEventArgs(midiNoteToSideLED(msg.Pitch), msg.Velocity));
            }

        }

        public int midiNoteToSideLED(Pitch p)
        {
            switch (p)
            {
                case Pitch.F6:
                    return 89;
                case Pitch.G5:
                    return 79;
                case Pitch.A4:
                    return 69;
                case Pitch.B3:
                    return 59;
                case Pitch.CSharp3:
                    return 49;
                case Pitch.DSharp2:
                    return 39;
                case Pitch.F1:
                    return 29;
                case Pitch.G0:
                    return 19;
                default:
                    return 0;
            }
        }

        public int[] midiNoteToLed(Pitch p)
        {
            for (int x = 0; x <= 7; x++)
            {
                for (int y = 0; y <= 7; y++)
                {
                    if (notes[x, y] == p)
                    {
                        int[] r1 = { x, y };
                        return r1;
                    }
                }
            }
            int[] r2 = { 0, 0 };
            return r2;
        }

        public Pitch ledToMidiNote(int x, int y)
        {
            return notes[x, y];
        }

        public void clearAllLEDs()
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    setLED(x, y, 0);
                }
            }

            for (int ry = 0; ry < 8; ry++)
            {
                setSideLED(ry, 0);
            }

            for (int tx = 1; tx < 9; tx++)
            {
                setTopLEDs(tx, 0);
            }
        }

        public void setTopLEDs(int x, int velo)
        {
            byte[] data = { 240, 0, 32, 41, 2, 24, 10, Convert.ToByte(103 + x), Convert.ToByte(velo), 247 };
            targetOutput.SendSysEx(data);
        }

        public void setSideLED(int y, int velo)
        {
            targetOutput.SendNoteOn(Channel.Channel1, rightLEDnotes[y], velo);
        }
        public void setLED(int x, int y, int velo)
        {
            try
            {
                targetOutput.SendNoteOn(Channel.Channel1, notes[x, y], velo);
            }
            catch (Midi.DeviceException)
            {
                Console.WriteLine("<< LAUNCHPAD.NET >> Midi.DeviceException");
                throw;
            }
        }

        /// <summary>
        /// Returns all connected and installed Launchpads.
        /// </summary>
        /// <returns>Returns LaunchpadDevice array.</returns>
        public LaunchpadDevice[] getConnectedLaunchpads()
        {
            List<LaunchpadDevice> tempDevices = new List<LaunchpadDevice>();

            foreach (InputDevice id in Midi.InputDevice.InstalledDevices)
            {
                foreach (OutputDevice od in Midi.OutputDevice.InstalledDevices)
                {
                    if (id.Name == od.Name)
                    {
                        if (id.Name.ToLower().Contains("launchpad"))
                        {
                            tempDevices.Add(new LaunchpadDevice(id.Name));
                        }
                    }
                }
            }

            return tempDevices.ToArray();
        }

        public void onControlChange(Midi.ControlChangeMessage e)
        {
            if (OnLaunchpadCCKeyPressed != null)
            {
                OnLaunchpadCCKeyPressed(this, new LaunchpadCCKeyEventArgs(int.Parse(e.Control.ToString()),e.Value));
            }
        }

        public bool connect(LaunchpadDevice device)
        {
            foreach (InputDevice id in Midi.InputDevice.InstalledDevices)
            {
                if (id.Name.ToLower() == device._midiName.ToLower())
                {
                    targetInput = id;
                    id.Open();
                    targetInput.ControlChange += new InputDevice.ControlChangeHandler(onControlChange);
                    targetInput.NoteOn += new InputDevice.NoteOnHandler(midiPress);
                    targetInput.StartReceiving(null);
                }
            }
            foreach (OutputDevice od in Midi.OutputDevice.InstalledDevices)
            {
                if (od.Name.ToLower() == device._midiName.ToLower())
                {
                    targetOutput = od;
                    od.Open();
                }
            }

            return true;
        }

        public bool disconnect(LaunchpadDevice device)
        {
            if (targetInput.IsOpen && targetOutput.IsOpen)
            {
                targetInput.StopReceiving();
                targetInput.Close();
                targetOutput.Close();
            }
            return !targetInput.IsOpen && !targetOutput.IsOpen;
        }

        public class LaunchpadDevice
        {
            public string _midiName;

            public LaunchpadDevice(string name)
            {
                _midiName = name;
            }
        }
    }
}
#pragma warning restore IDE1006 // 命名样式