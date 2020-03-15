# Description

Do you want to distribute a software to a client and need to release updates for him but you don't want to work on an installer or login remotely to update his software?

This small application downloads files from a website by accessing file.php which reads all the files in a remote URL and downloads them into a destination folder.

# Requirements

- Server that runs PHP 5.6+
- .Net Framework 4.6 installed on a PC

# Installation

1. Upload the files.php to a remote server (any shared hosting can work)
2. Copy the binaries to a folder on the user's PC as well as the 2 txt files:
   1. "destination directory.txt"
   2. "remote url.txt"
3. Modify the txt files to set the destination directory and the remote URL location

# Configuration

Example of destination directory location:
C:\Users\User\Desktop\tmp

Example of remote URL location (where you've uploaded files.php):
https://your-domain.com/updater/

# How to use?

Start the executable and click on "Start Update". 

**NB: you might need administrator permissions depending on your destination directory**