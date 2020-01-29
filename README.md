# TremAn3

![treman logo](TremAn3/Assets/StoreLogo.scale-200.png?raw=true)

TremaAn app measures frequency of tremor (e.g. Parkinsonian tremor) from video.

![treman logo](NoApp/Store/screenshot03.png?raw=true )

## Features

- Opens and plays videos
- Counts frequency from video using [CoM algorithm](https://github.com/tesar-tech/treman_algorithms) (Center of Motion)
- Enables ROI selection
  - ROI size can be adjusted. (use arrow keys for small change, PgDown/Up for large)
- Displays plot with power spectral density
- Displays plot with CoM values
- Export values to .csv format
- Set time range for computation
- Controlled by touch, keyboard, mouse or pen

## Availability

Available in Microsoft Store.

<a href='//www.microsoft.com/store/apps/9nl11tzlsfdp?cid=storebadge&ocid=badge'><img src='https://assets.windowsphone.com/13484911-a6ab-4170-8b7e-795c1e8b4165/English_get_L_InvariantCulture_Default.png' alt='English badge' width="128" /></a>

Alternatily it can be downloaded as msix package. Instructions [here](https://treman3.azurewebsites.net/).


[![Build Status](https://dev.azure.com/tesarj13/TremAn/_apis/build/status/tesar-tech.TremAn3?branchName=develop)](https://dev.azure.com/tesarj13/TremAn/_build/latest?definitionId=6&branchName=develop)

## Changelog

### 202001

- ROI size is adjustable by textboxes.
- What's new dialog in the app
- ROI implementation - works better on small details and insignificant movement.
  - ROI is customizable (size and position changes)
  - Also works with resolution reduction (speed boost).
  - Works with window size changes.
- Store badge in README  
- In app notifications
- CSV export
- Support for x64 (fixes ffmpeginterop package)
- FFmpegInteropX in nuget package
