@echo off

ECHO This process running as Windows user: %USERNAME%
ECHO(

cd C:\PRM\Cdr\WoahRawDataTransfer
PowerShell.exe -File C:\PRM\Cdr\CdrWoahRawDataTransfer\BayClinicEmrSftp.ps1

IF ERRORLEVEL 1 (
  ECHO Retrieval error, aborting batch processing
  ECHO(
  EXIT /b
)

EXIT /b
rem remove the above

ECHO Retrieval completed successfully
ECHO(

cd C:\PRM\Cdr\CdrRawDataExtract
C:\PRM\Cdr\CdrWohRawDataExtract\CdrExtractConsoleApp.exe

IF ERRORLEVEL 1 (
  ECHO Extraction error, aborting batch processing
  ECHO(
  EXIT /b
)

ECHO Extraction completed successfully
ECHO(

cd C:\PRM\Cdr\WoahCdrAggregation
C:\PRM\Cdr\CdrWohAggregation\WoahCdrAggregationConsoleApp.exe

IF ERRORLEVEL 1 (
  ECHO Aggregation error, aborting batch processing
  ECHO(
  EXIT /b
)
 
echo Aggregation completed successfully
echo(
