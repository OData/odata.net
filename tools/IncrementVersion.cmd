@echo off

powershell.exe -executionpolicy remotesigned -File %~dp0IncrementVersion.ps1 %*