# BIDashboard

A cross-platform Business Intelligence dashboard built with Xamarin.Forms and C#, giving users a consolidated view of vehicle sales data with interactive charts, filters, and trend analysis.

## Overview

BIDashboard reads sales data from an embedded Excel spreadsheet and caches it locally as JSON, so data remains available even after the app is closed. Users can analyse trends, compare results by geographical region, and drill into a specific vehicle's sales activity through interactive charts and filters, all backed by asynchronous data loading so the UI never freezes, regardless of dataset size.

## Features

- Quarterly sales bar chart and vehicle distribution pie chart
- Detailed sales ListView with granular filtering
- Filter by year, region, vehicle type, quarter, and quantity range
- Asynchronous data loading (`LoadDataAsync`, `FetchSalesAsync`) via background threads, keeping the UI responsive during load
- JSON caching on first launch — falls back to reading the embedded Excel file only when no cache exists
- Data bound via `ObservableCollection`, so charts and lists update automatically on filtering, aggregation, or reload with no manual refresh

## Tech Stack

C#, Xamarin.Forms (Android & iOS), Excel/JSON data handling

## My Role

Designed and built the full application solo, data loading architecture, async/concurrency handling, data models, filtering logic, and chart/list UI.

## How to Run

Requirements: Visual Studio 2022 with the Xamarin/.NET MAUI mobile development workload installed.

Clone the repo, open `BIDashboard.sln` in Visual Studio, select either the Android or iOS startup project, and run on an emulator or connected device.
