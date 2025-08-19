#!/bin/bash
cd Wyvern.API || exit

dotnet restore

dotnet build

dotnet run
