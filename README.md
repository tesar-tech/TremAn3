# TremAn3

![treman logo](TremAn3/Assets/StoreLogo.scale-200.png?raw=true)

TremaAn app measures frequency of tremor (e.g. Parkinsonian tremor) from video.

![treman app](NoApp/Store/appMetadata/en-us/baseListing/images/Screenshot/screenshot01.png?raw=true )

## Features

- Opens and plays videos
  - Can be opened by drag&drop
- Counts frequency from video using [CoM algorithm](https://github.com/tesar-tech/treman_algorithms) (Center of Motion)
- Enables ROI selection
  - ROI size can be adjusted. (use arrow keys for small change, PgDown/Up for large)
- Multiple ROIs for measuring different parts of the video (e.g left and right hand)
  - Multiple plots with the option to hide them
- Displays plot with power spectral density
- Displays plot with CoM values
- Export values to .csv format
- Set time range for computation
- Controlled by touch, keyboard, mouse or pen
- Basic info about video in the title of window

## Availability

Available in Microsoft Store.

<a href='//www.microsoft.com/store/apps/9nl11tzlsfdp?cid=storebadge&ocid=badge'><img src='https://assets.windowsphone.com/13484911-a6ab-4170-8b7e-795c1e8b4165/English_get_L_InvariantCulture_Default.png' alt='English badge' width="128" /></a>

[![Build Status](https://dev.azure.com/tesarj13/TremAn/_apis/build/status/master-store?branchName=master)](https://dev.azure.com/tesarj13/TremAn/_build/latest?definitionId=8&branchName=master)

Alternatily it can be downloaded as msix package. Instructions [here](https://treman3.azurewebsites.net/).


[![Build Status](https://dev.azure.com/tesarj13/TremAn/_apis/build/status/develop-cd?branchName=develop)](https://dev.azure.com/tesarj13/TremAn/_build/latest?definitionId=7&branchName=develop)

## Changelog

### 2020-06

- Computation could be canceled.
  - The button isn't disabled, the text is changed to `Cancel`.
  - It works with the `Esc` key.
- Now you can open video by drag&drop.
  - When an unsupported file type is dropped, a notification with info will pop up.
  - When dropping multiple files, the first supported will be opened.
- Info about video file in title of the app window.
  - Name, resolution, frame rate, size in MB.

### 2020-04

- Time range selector is within playback slider.
  - More intuitive placement.
- Secondary slider is added to proximity with CoM plots.
  - Helps to understand relation between CoM plots and video.
  - Only shows the selected time range (same as plots).
- Vertical line in CoM plots to view correlation between video and CoM movement.
  - Line follows video time position.
  - Current video position is visibile in plots.
- Mild flash on `Count freq` button as a warning of obsolete results.
  - When plots are displayed, and ROI is moved it isn't in correlation anymore. So plot disappears and button starts flashing.
- Buttons for making plots bigger.
- Removes unnecessary buttons from playback area.
- `No data` label for plots without data.

### 2020-03

- Multiple ROIs in one measurement.
- Multiple lines in plots (one for every ROI)
- ROI is in-place editable, has button for closing.
- Export values to CSV. Just visible ones.
- Better column names in exported files (with info about ROI).
- Fixes wrong filename on exported csvs.
- Exports CoMX and CoMY in different files.
- Freq Counter is opened by default (after video is loaded).
- X-Axis in CoM plots are in seconds (not frame number).
  - It also respects the time range.
- Lines on plots are thinner.

### 2020-01

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
