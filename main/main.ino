#define ENABLE_GxEPD2_GFX 0 // we won't need the GFX base class
#include <GxEPD2_BW.h>
// Online tool for converting images to byte arrays: 
// https://javl.github.io/image2cpp/
#include "config.h"
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>

// Instantiate the GxEPD2_BW class for our display type 
GxEPD2_BW<GxEPD2_750_T7, GxEPD2_750_T7::HEIGHT / 2>
display(GxEPD2_750_T7(15, 4, 2, 5)); // https://github.com/ZinggJM/GxEPD2/blob/2d1474910274148dc009ce65e55a295aeeb29d02/examples/GxEPD2_Example/GxEPD2_display_selection.h#L42

// EPD parameters
static const uint16_t input_buffer_pixels = 800; // may affect performance
static const uint16_t max_row_width = 800; // for up to 7.5" display 800x480
static const uint16_t max_palette_pixels = 256; // for depth <= 8
uint8_t input_buffer[3 * input_buffer_pixels]; // up to depth 24
uint8_t output_row_mono_buffer[max_row_width / 8]; // buffer for at least one row of b/w bits
uint8_t output_row_color_buffer[max_row_width / 8]; // buffer for at least one row of color bits
uint8_t mono_palette_buffer[max_palette_pixels / 8]; // palette buffer for depth <= 8 b/w
uint8_t color_palette_buffer[max_palette_pixels / 8]; // palette buffer for depth <= 8 c/w

void setup() {
  Serial.begin(115200);
  Serial.setTimeout(20000);

  // Connect to wifi
  connectToWifi();
  delay(1000);

  // Download Photo
  char* fileName = "tempimage";
  downloadImage(fileName, imageUrl);

  // Disconnect wifi as we no longer need it
  WiFi.disconnect(true);

  // Draw Image
  display.init(115200, true, 2, false);
  display.clearScreen();
  delay(1000);
  
  drawBitmapFromSpiffs(fileName, 0, 0, false);
  Serial.println("Image drawn");
  display.powerOff();

  // Go To Sleep
  Serial.println("I'm awake, but I'm going into deep sleep mode");
  ESP.deepSleep(delayInMicroseconds);
}

void loop() {
  
}

void connectToWifi() {
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.println("Connecting to wifi...");
    delay(500);
  }
  Serial.println("");
  Serial.print("IP Address: ");
  Serial.println(WiFi.localIP());
}

void downloadImage(const char *fileName, const char *url) {
  if (!SPIFFS.begin()) {
    Serial.println("SPIFFS initialisation failed!");
    while (1) yield(); // Stay here twiddling thumbs waiting
  }

  Serial.println("Removing previous images...");
  SPIFFS.format();
  File f = SPIFFS.open(fileName, "w");
  if (f) {
    HTTPClient http;
    http.setTimeout(20000);
    http.begin(url);
    Serial.print("Making request: ");
    Serial.println(url);
    int httpCode = http.GET();
    Serial.println(httpCode);
    if (httpCode > 0) {
      if (httpCode == HTTP_CODE_OK) {
        http.writeToStream(&f);
        Serial.println("File downloaded.");
      }
    } else {
      Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }

    http.end();
  }

  f.close();
}

// EPD image processing functions
// Taken from https://github.com/acrobotic/Ai_Demos_ESP8266/blob/571726331b5b1f463c4f1b66b68df3ffa25bc030/epd_slideshow/epd_slideshow.ino#L263/
// TODO: Because of the processing we're doing in the cloud, this could probably be simplified.
void drawBitmapFromSpiffs(const char *filename, int16_t x, int16_t y, bool with_color)
{
  fs::File file;
  bool valid = false; // valid format to be handled
  bool flip = true; // bitmap is stored bottom-to-top
  uint32_t startTime = millis();
  if ((x >= display.width()) || (y >= display.height())) return;
  Serial.println();
  Serial.print("Loading image '");
  Serial.print(filename);
  Serial.println('\'');
#if defined(ESP32)
  file = SPIFFS.open(String("/") + filename, "r");
#else
  file = SPIFFS.open(filename, "r");
#endif
  if (!file)
  {
    Serial.print("File not found");
    return;
  }
  // Parse BMP header
  if (read16(file) == 0x4D42) // BMP signature
  {
    uint32_t fileSize = read32(file);
    uint32_t creatorBytes = read32(file);
    uint32_t imageOffset = read32(file); // Start of image data
    uint32_t headerSize = read32(file);
    uint32_t width  = read32(file);
    uint32_t height = read32(file);
    uint16_t planes = read16(file);
    uint16_t depth = read16(file); // bits per pixel
    uint32_t format = read32(file);

    Serial.print("File size: "); Serial.println(fileSize);
    Serial.print("Image Offset: "); Serial.println(imageOffset);
    Serial.print("Header size: "); Serial.println(headerSize);
    Serial.print("Bit Depth: "); Serial.println(depth);
    Serial.print("Image size: ");
    Serial.print(width);
    Serial.print('x');
    Serial.println(height);
    
    if ((planes == 1) && ((format == 0) || (format == 3))) // uncompressed is handled, 565 also
    {
      
      // BMP rows are padded (if needed) to 4-byte boundary
      uint32_t rowSize = (width * depth / 8 + 3) & ~3;
      if (depth < 8) rowSize = ((width * depth + 8 - depth) / 8 + 3) & ~3;
      if (height < 0)
      {
        height = -height;
        flip = false;
      }
      uint16_t w = width;
      uint16_t h = height;
      if ((x + w - 1) >= display.width())  w = display.width()  - x;
      if ((y + h - 1) >= display.height()) h = display.height() - y;
      if (w <= max_row_width) // handle with direct drawing
      {
        valid = true;
        uint8_t bitmask = 0xFF;
        uint8_t bitshift = 8 - depth;
        uint16_t red, green, blue;
        bool whitish, colored;
        if (depth == 1) with_color = false;
        if (depth <= 8)
        {
          if (depth < 8) bitmask >>= depth;
          //file.seek(54); //palette is always @ 54
          file.seek(imageOffset - (4 << depth)); // 54 for regular, diff for colorsimportant
          for (uint16_t pn = 0; pn < (1 << depth); pn++)
          {
            blue  = file.read();
            green = file.read();
            red   = file.read();
            file.read();
            whitish = with_color ? ((red > 0x80) && (green > 0x80) && (blue > 0x80)) : ((red + green + blue) > 3 * 0x80); // whitish
            colored = (red > 0xF0) || ((green > 0xF0) && (blue > 0xF0)); // reddish or yellowish?
            if (0 == pn % 8) mono_palette_buffer[pn / 8] = 0;
            mono_palette_buffer[pn / 8] |= whitish << pn % 8;
            if (0 == pn % 8) color_palette_buffer[pn / 8] = 0;
            color_palette_buffer[pn / 8] |= colored << pn % 8;
          }
        }
        uint32_t rowPosition = flip ? imageOffset + (height - h) * rowSize : imageOffset;
        for (uint16_t row = 0; row < h; row++, rowPosition += rowSize) // for each line
        {
          uint32_t in_remain = rowSize;
          uint32_t in_idx = 0;
          uint32_t in_bytes = 0;
          uint8_t in_byte = 0; // for depth <= 8
          uint8_t in_bits = 0; // for depth <= 8
          uint8_t out_byte = 0xFF; // white (for w%8!=0 boarder)
          uint8_t out_color_byte = 0xFF; // white (for w%8!=0 boarder)
          uint32_t out_idx = 0;
          file.seek(rowPosition);
          for (uint16_t col = 0; col < w; col++) // for each pixel
          {
            // Time to read more pixel data?
            if (in_idx >= in_bytes) // ok, exact match for 24bit also (size IS multiple of 3)
            {
              in_bytes = file.read(input_buffer, in_remain > sizeof(input_buffer) ? sizeof(input_buffer) : in_remain);
              in_remain -= in_bytes;
              in_idx = 0;
            }
            switch (depth)
            {
              case 24:
                blue = input_buffer[in_idx++];
                green = input_buffer[in_idx++];
                red = input_buffer[in_idx++];
                whitish = with_color ? ((red > 0x80) && (green > 0x80) && (blue > 0x80)) : ((red + green + blue) > 3 * 0x80); // whitish
                colored = (red > 0xF0) || ((green > 0xF0) && (blue > 0xF0)); // reddish or yellowish?
                break;
              case 16:
                {
                  uint8_t lsb = input_buffer[in_idx++];
                  uint8_t msb = input_buffer[in_idx++];
                  if (format == 0) // 555
                  {
                    blue  = (lsb & 0x1F) << 3;
                    green = ((msb & 0x03) << 6) | ((lsb & 0xE0) >> 2);
                    red   = (msb & 0x7C) << 1;
                  }
                  else // 565
                  {
                    blue  = (lsb & 0x1F) << 3;
                    green = ((msb & 0x07) << 5) | ((lsb & 0xE0) >> 3);
                    red   = (msb & 0xF8);
                  }
                  whitish = with_color ? ((red > 0x80) && (green > 0x80) && (blue > 0x80)) : ((red + green + blue) > 3 * 0x80); // whitish
                  colored = (red > 0xF0) || ((green > 0xF0) && (blue > 0xF0)); // reddish or yellowish?
                }
                break;
              case 1:
              case 4:
              case 8:
                {
                  if (0 == in_bits)
                  {
                    in_byte = input_buffer[in_idx++];
                    in_bits = 8;
                  }
                  uint16_t pn = (in_byte >> bitshift) & bitmask;
                  whitish = mono_palette_buffer[pn / 8] & (0x1 << pn % 8);
                  colored = color_palette_buffer[pn / 8] & (0x1 << pn % 8);
                  in_byte <<= depth;
                  in_bits -= depth;
                }
                break;
            }
            if (whitish)
            {
              // keep white
            }
            else if (colored && with_color)
            {
              out_color_byte &= ~(0x80 >> col % 8); // colored
            }
            else
            {
              out_byte &= ~(0x80 >> col % 8); // black
            }
            if ((7 == col % 8) || (col == w - 1)) // write that last byte! (for w%8!=0 boarder)
            {
              output_row_color_buffer[out_idx] = out_color_byte;
              output_row_mono_buffer[out_idx++] = out_byte;
              out_byte = 0xFF; // white (for w%8!=0 boarder)
              out_color_byte = 0xFF; // white (for w%8!=0 boarder)
            }
          } // end pixel
          uint16_t yrow = y + (flip ? h - row - 1 : row);
          display.writeImage(output_row_mono_buffer, output_row_color_buffer, x, yrow, w, 1);
        } // end line
        Serial.print("loaded in "); Serial.print(millis() - startTime); Serial.println(" ms");
        display.refresh();
      }
    }
  }
  file.close();
  if (!valid)
  {
    Serial.println("bitmap format not handled.");
  }
}

uint16_t read16(fs::File& f)
{
  // BMP data is stored little-endian, same as Arduino.
  uint16_t result;
  ((uint8_t *)&result)[0] = f.read(); // LSB
  ((uint8_t *)&result)[1] = f.read(); // MSB
  return result;
}

uint32_t read32(fs::File& f)
{
  // BMP data is stored little-endian, same as Arduino.
  uint32_t result;
  ((uint8_t *)&result)[0] = f.read(); // LSB
  ((uint8_t *)&result)[1] = f.read();
  ((uint8_t *)&result)[2] = f.read();
  ((uint8_t *)&result)[3] = f.read(); // MSB
  return result;
}
