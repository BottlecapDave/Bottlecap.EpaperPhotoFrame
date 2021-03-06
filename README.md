# Bottlecap.EPaperPhotoFrame

Always wanting a digital photo frame, but never being able to justify having such a device on all of the time just to display photos I never got one. Enter EPaper, which uses basically no power when displaying images. 

There are a few examples on YouTube of people who created their own DIY Epaper displays. 

[One](https://youtu.be/tc0hM_71ziU) person had their photos read off an SD card, which is great as no internet connection is required for the device to work. However the photos required pre-processing before being loaded onto the SD card due to the limited memory of the device. This could be seen as a blocker for non-technical people as they might not have the know how to change the images.

[Another one](https://youtu.be/p3jXovhnfCU) saw the ESP8266 run a webserver, which supported photos being uploaded directly to the device. This meann't that the required pre-processing could be done on the client doing the uploading. However, because the web server was on the device, it meant that it could never go to sleep as you couldn't anticipate when the user wanted to change the pictures. 

So what is different with my solution?

Well, I have moved the processing of the images into a cloud function and have the ESP8266 download it's images from here. I've built the cloud functions to easily be expandable.

Advantages
- Photo processing is achieved automatically via cloud functions. This removes the blocker for non-technical people as any photo will be automatically adjusted to work on the EPaper display.
- Because the processing is done in a cloud function, external power consumption will only be needed when a new image is loaded. The monetary cost will also be nothing (as long as you're not refreshing your image 30 seconds).
- Like other solutions, this solution runs on an ESP8266. Because the processing is done in the cloud, this device is in low power mode for the majority of time. This results in a device that can run off batteries for months.

Disadvantages
- Requires an internet connection (unless the cloud functionality is moved to a local server - raspberry pi anyone?)

## Cloud Functions

The code base for cloud functions can be found within `backend/src`.

### Examples

The following cloud function examples are provided. The codebase has been structured so you only need to provide an `IImageProvider` for, well, retrieving the image and the cloud function entry point. This makes it easy to create your own.

#### Unsplash

This cloud function downloads a random photo meeting a given query from [Unsplash](https://unsplash.com/). The following arguments should be provided when setting it up

| Env Variable          | Description |
|-----------------------|-------------|
| UNSPLASH_ACCESS_TOKEN | The unsplash access token. You'll need to sign up for an account. |
| UNSPLASH_QUERY        | The query to apply to the random picture |
| UNSPLASH_ORIENTATION  | The orientation of the pictures to retrieve. This should be set to `landscape` or `portrait` depending on the orientation of your photo frame |

#### XKCD

This cloud function downloads the latest [XKCD](https://xkcd.com/) comic.

### Installation

TODO - Create terraform script.

## Epaper Code

The code for retrieving and displaying images on the epaper device can be found in `device`. 

## Hardware

- [ESP8266 Device](https://www.amazon.co.uk/gp/product/B06Y1ZPNMS/ref=ppx_yo_dt_b_asin_title_o03_s00?ie=UTF8&psc=1)
- [7.5 inch Epaper display](https://www.amazon.co.uk/gp/product/B075R4QY3L/ref=ppx_yo_dt_b_asin_title_o02_s00?ie=UTF8&psc=1)

To connect the devices, you'll need to connect the following

| EPaper Connection | ESP Connection |
|-------------------|----------------|
| BUSY              | D1             |
| RST               | D4             |
| DC                | D2             |
| CS                | D8             |
| CLK               | D5             |
| DIN               | D7             |
| GND               | G              |
| VCC               | 3V             |

### Prerequisites

- [Arduino IDE](https://www.arduino.cc/en/Main/Software)
- [ESP8266 Arduino IDE](https://randomnerdtutorials.com/how-to-install-esp8266-board-arduino-ide/)
- [Arduino drivers](https://github.com/nodemcu/nodemcu-devkit/tree/master/Drivers)
- [GxEPD2 Library](https://github.com/ZinggJM/GxEPD2)

### Config

In order for it to work, you'll need to create a file called `config.h` which includes the sensitive information that needs to be configured by you.

```c++
const char* ssid PROGMEM = "MY_WIFI";
const char* password PROGMEM = "PASSWORD";
const char* imageUrl PROGMEM = "http://your-cloud-function-url?width=480&height=800"; // https doesn't work at the moment :(
const uint16_t hoursBetweenUpdates PROGMEM = 6;
```

### Installation

- Open up the code in `main` with Arduino IED
- Connect the ESP8266 to your computer
- Upload the code to the ESP8266
- Connect `D0` and `RST` pins to support the arduino going into deep sleep.