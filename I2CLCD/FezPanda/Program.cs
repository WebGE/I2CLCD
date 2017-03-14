using System;
using System.Threading;

using testMicroToolsKit.Hardware.Displays;


namespace FezPanda
{
    public class Program
    {
        public static void Main()
        {
            byte InitJauge = 0x5A;
            Int16 Freq = 100;

            // MIDAS MC21605E6W : http://www.farnell.com/datasheets/1722538.pdf
            // I2CLcd : http://webge.github.io/LCDI2C/
            I2CLcd lcd = new I2CLcd(I2CLcd.LcdManufacturer.BATRON, Freq);

            lcd.Init(); lcd.ClearScreen();

            lcd.PutString(2, 0, "Hello");
            lcd.PutChar(11, 0, 0x4E);
            lcd.PutString(3, 1, "Batron");

            for (byte w = InitJauge; w < 0x60; w++)
                lcd.PutChar((byte)(w - 0x51), 1, w);

            while (true)
            {
                lcd.PutChar(14, 0, 0x11); lcd.PutChar(15, 0, 0x21); lcd.PutChar(13, 0, 0x4C);
                Thread.Sleep(200);
                lcd.PutChar(14, 0, 0x21); lcd.PutChar(15, 0, 0x11); lcd.PutChar(13, 0, 0x4B);
                Thread.Sleep(200);

                lcd.PutChar(0, 0, (byte)InitJauge);
                InitJauge++;
                if (InitJauge > 0x5F) InitJauge = 0x5A;
            }
        }
    }
}
