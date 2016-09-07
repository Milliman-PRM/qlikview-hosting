@echo off

ECHO This process running as Windows user: %USERNAME%
ECHO(

cd C:\PRM\Cdr\CdrWohRawDataTransfer
PowerShell.exe -File C:\PRM\Cdr\CdrWohRawDataTransfer\BayClinicEmrSftp.ps1

IF ERRORLEVEL 1 (
  ECHO Retrieval error, aborting batch processing
  ECHO(
  EXIT /b
)

ECHO Retrieval completed successfully
ECHO(

cd C:\PRM\Cdr\CdrWohRawDataExtract
C:\PRM\Cdr\CdrWohRawDataExtract\CdrExtractConsoleApp.exe

IF ERRORLEVEL 1 (
  ECHO Extraction error, aborting batch processing
  ECHO(
  EXIT /b
)

ECHO Extraction completed successfully
ECHO(

cd C:\PRM\Cdr\CdrWohAggregation
C:\PRM\Cdr\CdrWohAggregation\WoahCdrAggregationConsoleApp.exe

IF ERRORLEVEL 1 (
  ECHO Aggregation error, aborting batch processing
  ECHO(
  EXIT /b
)
 
echo Aggregation completed successfully
echo(
