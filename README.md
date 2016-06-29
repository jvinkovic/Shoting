# Shoting
Screenshot taker - takes screenshots and sends them to mail.

Working version is in "bin/Release" folder.
Configuration is in config.txt

If there is no internet connection screenshots are saved where program is started.

### How It Works?
When started, program is copied in temp folder and started with configuration from config.txt
Original program that was started is closed then and only second instance is running with parameters from configuration.

If there is no internet connection to send mail, screenshots are saved at place where initial starting of program was (if it can be saved there). When internet connection is restored, screenshots are again sent to mail.

Great for starting from usb keys and then unplugging them, while program is still running without any problems.
