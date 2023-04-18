@echo off

powershell.exe -executionpolicy remotesigned -File %~dpn0.ps1 %*