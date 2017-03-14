using manipulatorDriver;
using System;
using System.Globalization;
using System.Threading;

namespace ManipulatorDriver
{
    public class E2JManipulator : Observer
    {

        #region Enums
        public enum GrabE { Closed, Open };
        #endregion

        #region Fields and Properties

        private readonly SerialComm port;
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double A { get; private set; }
        public double B { get; private set; }
        public GrabE Grab { get; private set; }
        private ResponsiveCommand lastRequest;
        #endregion

        public E2JManipulator()
        {
            port = new SerialBuilder().Build();
            lastRequest = ResponsiveCommand.INVALID;
            port.Subscribe(this);
        }

        public void Connect(string portName)
        {
            // TODO: Launching servo seems a pretty good idea
            port.OpenPort(portName);
            Where();
        }

        public void Disconnect()
        {
            port.ClosePort();
        }
        /// <summary>
        /// Moves the end of the hand to a position away from the current position by the distance specified in X, Y and Z directions. (Joint interpolation)
        /// </summary>
        /// <param name="travelDistanceInX">Specify the amount that you want to move in X direction from the current position.</param>
        /// <param name="travelDistanceInY">Specify the amount that you want to move in Y direction from the current position.</param>
        /// <param name="travelDistanceInZ">Specify the amount that you want to move in Z direction from the current position.</param>
        public void Draw(double travelDistanceInX, double travelDistanceInY, double travelDistanceInZ)
        {
            port.Write($"DW {travelDistanceInX},{travelDistanceInY},{travelDistanceInZ}");
        }

        /// <summary>
        /// Close the grip of hand.
        /// </summary>
        public void GrabClose()
        {
            port.Write("GC");
        }

        /// <summary>
        /// Opens the grip of the hand.
        /// </summary>
        public void GrabOpen()
        {
            port.Write("GO");
        }
        /// <summary>
        /// Defines the gripping force to be applied the motor-operated hand is closed and opened.
        /// </summary>
        /// <param name="startingGrippingForce">Specify necessary gripping force in integer value to activate hand open or close.</param>
        /// <param name="retainedGrippingForce">Specify necessary gripping force in integer value to maintain hand open or close.</param>
        /// <param name="startingGrippingForceRetentionTime">Specify time continuing starting gripping force in integer value.</param>
        public void GripPressure(double startingGrippingForce, double retainedGrippingForce,
                                 double startingGrippingForceRetentionTime)
        {
            port.Write($"GP {startingGrippingForce},{retainedGrippingForce},{startingGrippingForceRetentionTime}");
        }

        /// <summary>
        /// Defines the current coordinates as the specified position.
        /// </summary>
        /// <param name="positionNumber">Specify the position number to be registered [0, 999]. Registers the current position to the user-defined origin in case of zero.</param>
        public void Here(double positionNumber)
        {
            port.Write($"HE {positionNumber}");
        }

        /// <summary>
        /// Interrupts the motion of the robot and the operation of the program.
        /// </summary>
        public void Halt()
        {
            port.Write("HLT");
        }

        /// <summary>
        /// Defines the current location and the attitude as origin point.
        /// </summary>
        /// <param name="originSettingAproach">Specify the method to set origin in integer value:  
        /// 0: Mechanical sstopper origin;
        /// 1: Jig origin;
        /// 2: User-defined origin</param>
        public void Home(double originSettingAproach)
        {
            port.Write($"HO {originSettingAproach}");
        }

        /// <summary>
        /// Moves the robot to a predifined position with a position number greater than the current one.
        /// </summary>
        public void Ip()
        {
            port.Write("IP");
        }

        /// <summary>
        /// Overwrites the current position by adding +/- 360 degrees to the joint position of the R-axis. This is done when you want to use shortcut control of the R-axis, or when you want to use endless control.
        /// </summary>
        /// <param name="number">+1: adds 360 degrees to the current joint position on the R-axis; -1: Substract 360 to the current joint position on the R-axis</param>
        public void JointRollChange(double number)
        {
            port.Write($"JRC {number}");
        }

        /// <summary>
        /// Moves the robot continously through the predefined intermediate points between two specified position numbers.
        /// </summary>
        /// <param name="positionNumberA">Specify the top position number moving continuous. [1...999]</param>
        /// <param name="positionNumberB">Specify the last position number moving continuous. [1...999]</param>
        public void MoveContinuous(double positionNumberA, double positionNumberB)
        {
            port.Write($"MC {positionNumberA},{positionNumberB}");
        }

        /// <summary>
        /// Turns each joint the specified angle from the current position. (Joint interpolation)
        /// </summary>
        /// <param name="waistJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="shoulderJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="elbowJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="twistJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="pitchJointAngle">Specify relative amount of each joint turning from the current position.</param>
        /// <param name="rollJointAngle">Specify relative amount of each joint turning from the current position.</param>
        public void MoveJoint(double waistJointAngle, double shoulderJointAngle, double elbowJointAngle,
                               double twistJointAngle, double pitchJointAngle, double rollJointAngle)
        {
            port.Write($"MJ {waistJointAngle},{shoulderJointAngle},{elbowJointAngle},{twistJointAngle},{pitchJointAngle},{rollJointAngle}");
        }


        /// <summary>
        /// Moves the tip of hand to a position whose coordinates (position and angle) have been specified. (Joint interpolation)
        /// </summary>
        /// <param name="xCoordinateValue">Specify the position in XYZ coordinates (mm) of the robot. (Zero for default)</param>
        /// <param name="yCoordinateValue">Specify the position in XYZ coordinates (mm) of the robot. (Zero for default)</param>
        /// <param name="zCoordinateValue">Specify the position in XYZ coordinates (mm) of the robot. (Zero for default)</param>
        /// <param name="aTurnAngle">Specify the turning angle of roll and pitch joints in XYZ coordinates (degree) of the robot. (Zero for default)</param>
        /// <param name="bTurnAngle"></param>
        public void MovePosition(double xCoordinateValue, double yCoordinateValue, double zCoordinateValue, double aTurnAngle, double bTurnAngle)
        {
            port.Write($"MP {xCoordinateValue},{yCoordinateValue},{zCoordinateValue},{aTurnAngle},{bTurnAngle}");
        }

        public void MoveAway(double x, double y, double z, double a, double b)
        {
            ///////////     port.Write($"MP {x},{y},{z},{a},{b}");
        }


        /// <summary>
        /// Moves to the specified position with specified interpolation, specified speed, specified timer, and specified input and output signal.
        /// </summary>
        /// <param name="speed">Specify the interpolation speed to the destination position. [0...32767[ (Joint interpolation: %; Linear interpolation: mm/s</param>
        /// <param name="timer">Set timer at the destination position after the movement. [0...255]</param>
        /// <param name="outputOn">Set the output signal that turns ON. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="outputOff">Set the output signal that turns OFF. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="inputOn">Set the input waiting signal that turns ON. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="inputOff">Set the input waiting signal that turns OFF. [0...& FFFF]; 1: Setting; 0: Not setting</param>
        /// <param name="interpolation">Specify the interpolation mode to the destination position. [0: Joint interpolation; 1: Linear interpolation; 2: Circular interpolation]</param>
        /// <param name="xCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="yCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="zCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="aTurningAngle">Specify the turning angle around roll in XYZ coordinates (degree) of the robot. (0 for default)</param>
        /// <param name="bTurningAngle">Specify the turning angle around pitch in XYZ coordinates (degree) of the robot. (0 for default)</param>
        public void MovePlayback(double speed, double timer, double outputOn, double outputOff, double inputOn,
                                 double inputOff, double interpolation, double xCoordinate, double yCoordinate,
                                 double zCoordinate, double aTurningAngle, double bTurningAngle)
        {
            port.Write($"MPB {speed},{timer},{outputOn},{outputOff},{inputOn},{inputOff},{interpolation},{xCoordinate},{yCoordinate},{zCoordinate},{aTurningAngle},{bTurningAngle}");
        }

        /// <summary>
        /// Moves to the specified position with specified interpolation.
        /// </summary>
        /// <param name="interpolation">Specify the interpolation mode to the destination position. [0: Joint interpolation; 1: Linear interpolation; 2: Circular interpolation]</param>
        /// <param name="xCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="yCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="zCoordinate">Specify the location (mm) in XYZ coordinates of the robot. (Zero by default)</param>
        /// <param name="aTurningAngle">Specify the turning angle around roll in XYZ coordinates (degree) of the robot. (0 for default)</param>
        /// <param name="bTurningAngle">Specify the turning angle around pitch in XYZ coordinates (degree) of the robot. (0 for default)</param>
        public void MovePlaybackContinuous(double interpolation, double xCoordinate, double yCoordinate,
                                           double zCoordinate, double aTurningAngle, double bTurningAngle)
        {
            port.Write($"MPC {interpolation},{xCoordinate},{yCoordinate},{zCoordinate},{aTurningAngle},{bTurningAngle}");
        }

        /// <summary>
        /// Moves the tip of hand through the predefined intermediate positions in circular interpolation.
        /// </summary>
        /// <param name="positionNumberA">Specify the position on the circle. [1...999]</param>
        /// <param name="positionNumberB">Specify the position on the circle. [1...999]</param>
        /// <param name="positionNumberC">Specify the position on the circle. [1...999]</param>
        public void MoveR(double positionNumberA, double positionNumberB, double positionNumberC)
        {
            port.Write($"MR {positionNumberA},{positionNumberB},{positionNumberC}");
        }

        /// <summary>
        /// Moves to specified position in circulat interpolation.
        /// </summary>
        /// <param name="positionNumber">Specify the destination position. [1...999]</param>
        public void MoveRA(double positionNumber)
        {
            port.Write($"MRA {positionNumber}");
        }

        /// <summary>
        /// Moves the tip of hand to the specified position.
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        public void MoveStraight(double positionNumber)
        {
            port.Write($"MS {positionNumber}");
        }

        /// <summary>
        /// Moves the tip of hand to a position away from the specified position by the distance as specified in the tool direction. (Linear interpolation)
        /// </summary>
        /// <param name="positionNumber">Specify the destination position number in integer value. [1...999]</param>
        /// <param name="travelDistance">Specify the distance in tool direction from the specified position to the destination point. (Zero by default). [-3276,80...3276,70]</param>
        public void MoveToolStraight(double positionNumber, double travelDistance)
        {
            port.Write($"MTS {positionNumber},{travelDistance}");
        }

        /// <summary>
        /// Moves to the user-defined origin. (Joint interpolation)
        /// </summary>
        public void Origin()
        {
            port.Write($"OG");
        }

        /// <summary>
        /// Defines the number of grid points in the column and row directions for the specified pallet.
        /// </summary>
        /// <param name="palletNumber">Specify number of pallet using. [1...9]</param>
        /// <param name="numberOfColumnGridPoints">Set grid points of column of pallet. [1...32767]</param>
        /// <param name="numberOfRowGridPoints">Set grid points of row of pallet. [1...32767]</param>
        public void PalletAssign(double palletNumber, double numberOfColumnGridPoints, double numberOfRowGridPoints)
        {
            port.Write($"PA {palletNumber},{numberOfColumnGridPoints},{numberOfRowGridPoints}");
        }

        /// <summary>
        /// Clears the data of the specified position (s).
        /// </summary>
        /// <param name="positionNumberA">Specify position number deleting. [1...999]</param>
        public void PositionClear(double positionNumberA)
        {
            port.Write($"PC {positionNumberA}");
        }

        /// <summary>
        /// Reads the coordinates of the specified position and the open/close state of the hand. (Using RS-232C)
        /// </summary>
        /// <param name="positionNumber">Specify the position number that you want to read. [0...999] (If ommited, the current position number is valid.)</param>
        public void PosiotionRead(double positionNumber)
        {
            port.Write($"PR {positionNumber}");
        }

        /// <summary>
        /// Defines the moving velocity, first order time constant, acceleration/deceleration time, and continous path setting.
        /// </summary>
        /// <param name="movingSpeed">Set moving speed at linear or circulat interpolation. [0,01...650](mm/s)</param>
        public void SpeedDefine(double movingSpeed)
        {
            port.Write($"SD {movingSpeed}");
        }

        /// <summary>
        /// Sets the operating speed, acceleration or deceleration time and the continuous path setting.
        /// </summary>
        /// <param name="speedLevel">Set moving speed. [0...30]</param>
        public void Speed(double speedLevel)
        {
            port.Write($"SP {speedLevel}");
        }

        /// <summary>
        /// Establishes the distance between the hand mounting surface and the tip of hand.
        /// </summary>
        /// <param name="toolLength">Set the distance from the hand mounting surface to the tip of hand. [0...300](mm)</param>
        public void Tool(double toolLength)
        {
            port.Write($"TL {toolLength}");
        }

        /// <summary>
        /// Reads the coordinates of the current position and the open or close state of the hand. (Using RS-232C)
        /// </summary>
        private void Where()
        {
            port.Write("WH");
            lastRequest = ResponsiveCommand.WH;
        }

        /// <summary>
        /// Reads the tool length currently being established. (Using RS-232C)
        /// </summary>
        public void WhatTool()
        {
            port.Write($"WT");
        }

        private enum ResponsiveCommand
        {
            INVALID, WH
        };

        // TODO: Consider regex validation
        public void getNotified(string data)
        {
            switch (lastRequest)
            {
                case ResponsiveCommand.WH:
                    Parse(data);
                    lastRequest = ResponsiveCommand.INVALID;
                    break;
            }
        }

        public bool Parse(string position)
        {
            string[] splitted = position.Replace("+", "").Replace("\r", "").Split(',');
            if (splitted.Length != 10) return false;

            try
            {
                X = double.Parse(splitted[0], CultureInfo.InvariantCulture);
                Y = double.Parse(splitted[1], CultureInfo.InvariantCulture);
                Z = double.Parse(splitted[2], CultureInfo.InvariantCulture);
                A = double.Parse(splitted[3], CultureInfo.InvariantCulture);
                B = double.Parse(splitted[4], CultureInfo.InvariantCulture);
                Grab = splitted[9] == "O" ? GrabE.Open : GrabE.Closed;
            }
            catch (FormatException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            return true;
        }
    }
}
