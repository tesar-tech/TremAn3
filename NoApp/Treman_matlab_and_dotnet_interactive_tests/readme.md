# Treman - matlab and .net interactive test

This folder helps with converting some algorithms to .net.

`.ipynb` contains code in C#, that is easily outputed as matlab variables, thus can be checked easily.

`Welch.m` contains easy to understand implementation of cross power spectral density (cpsd) and welch spectra (pwelch in matlab). That function is reimplemented in .net as `Welch`.

`SCRIPT.m` matlab test of `Welch.m` function and `mscohere` (coherence) matlab functions