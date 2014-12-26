using System;
using System.Threading;
using Microsoft.SPOT;

using ToolBoxes;

namespace TestNetduinoI2CLCD
{
    public class Program
    {
        public static void Main()
        {   // Pour accéder au bus I2C, relier le LCD au connecteur TWI de la carte Tinkerkit.
            // Placer des résistances de rappel entre le +5V et les sorties SCL et SDA
            byte InitJauge = 0x5A; // Etat initial d'un caractère personalisé "jauge"
            UInt16 Freq = 100; // Fréquence d'horloge du bus I2C en kHz

            // Création d'un objet I2CLcd MIDAS MC21605E6W : http://www.farnell.com/datasheets/1722538.pdf
            // Documentation de la classe I2CLcd : http://webge.github.io/LCDI2C/
            I2CLcd lcd = new I2CLcd(I2CLcd.LcdManufacturer.MIDAS, Freq);

            // Initialisation du Lcd I2C
            lcd.Init(); lcd.ClearScreen();
            
            // Message          
            lcd.PutString(3, 0, "SSI...");
            lcd.PutChar(11, 0, 0x4E);
            lcd.PutString(2, 1, "Bonjour");
            // Jauges linéaires virtuelles
            for (byte w = InitJauge; w < 0x60; w++)
                lcd.PutChar((byte)(w - 0x51), 1, w);

            while (true)
            {
                // Démo Widgets
                lcd.PutChar(14, 0, 0x11); lcd.PutChar(15, 0, 0x21); lcd.PutChar(13, 0, 0x4C);
                Thread.Sleep(200);
                lcd.PutChar(14, 0, 0x21); lcd.PutChar(15, 0, 0x11); lcd.PutChar(13, 0, 0x4B);
                Thread.Sleep(200);

                // Démo jauge linéaire virtuelle
                lcd.PutChar(0, 0, (byte)InitJauge);
                InitJauge++;
                if (InitJauge > 0x5F) InitJauge = 0x5A;
            }
        }
    }
}
