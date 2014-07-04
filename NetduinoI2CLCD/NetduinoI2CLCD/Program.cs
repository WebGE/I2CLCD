using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using BatronLCD;

namespace NetduinoI2CLCD
{
    public class Program
    {
        public static void Main()
        {   // Pour accéder au bus I2C, relier le PCF8574 au connecteur TWI de la 
            // carte Tinkerkit. Placer des résistances de rappel entre le +5V et les sortie SCL et SDA
            byte y = 0x5A; // Etat initial d'un caractère personalisé "jauge"
            byte addLcd_I2C = 0x3B; // Adresse (7 bits) du Lcd I2C Batron
            UInt16 Freq = 400; // Fréquence d'horloge du bus I2C en kHz
 
            // Création d'un objet Lcd I2C Batron
            I2CLcd lcd = new I2CLcd(addLcd_I2C, Freq);

            // Initialisation du Lcd I2C
            lcd.Init();

            // Message
            lcd.ClearScreen();
            lcd.PutString(3, 0, "SSI...");
            lcd.PutChar(11, 0, 0x4E);
            lcd.PutString(2, 1, "Bonjour!!");
            // Jauges linéaires virtuelles
            for (byte w = 0x5a; w < 0x60; w++)
                lcd.PutChar((byte)(w - 0x51), 1, w);

            while (true)
            {
                // Démo Widgets
                lcd.PutChar(14, 0, 0x11);
                lcd.PutChar(15, 0, 0x21);
                lcd.PutChar(13, 0, 0x4C);
                Thread.Sleep(200);
                lcd.PutChar(14, 0, 0x21);
                lcd.PutChar(15, 0, 0x11);
                lcd.PutChar(13, 0, 0x4B);
                Thread.Sleep(200);

                // Démo jauge linéaire virtuelle
                lcd.PutChar(0, 0, (byte)y);
                y++;
                if (y > 0x5F) y = 0x5A;
            }
        }
    }
}
