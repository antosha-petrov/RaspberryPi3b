echo y | .\pscp -sftp -r -pw 38Parrots# ./* pi@raspberrypianton:Solution1/lighthouse

@echo off

@REM Use this if you have single server to login only, provide your sysyem address to puty.exe below.

@REM Provide your system's address to the putty.exe here
set putty="C:\Program Files\PuTTY\putty.exe"

@REM Provide your username@address here
set id=pi@raspberrypianton

@REM Provide your password here
set password=38Parrots#

%putty% -pw %password% %id%

@echo 