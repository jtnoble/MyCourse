using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MyCourse.Resources.Views
{
    public partial class TermReportPage : ContentPage
    {
        readonly string _filePathOrUri;

        public TermReportPage(string filePathOrUri, string termName)
        {
            InitializeComponent();

            _filePathOrUri = filePathOrUri;
            Title = string.IsNullOrEmpty(termName) ? "Term Report" : $"{termName} Report";
            TitleLabel.Text = Title;

            _ = LoadReportAsync();
        }

        async Task LoadReportAsync()
        {
            try
            {
                string text = null;

                if (string.IsNullOrEmpty(_filePathOrUri))
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        SummaryLabel.Text = "Report not available.";
                        CsvText.Text = string.Empty;
                    });
                    return;
                }

                // 1) If the path points to a real file, read it directly.
                if (File.Exists(_filePathOrUri))
                {
                    text = await File.ReadAllTextAsync(_filePathOrUri).ConfigureAwait(false);
                }
#if ANDROID
                else if (_filePathOrUri.StartsWith("content://", StringComparison.OrdinalIgnoreCase))
                {
                    // On Android, read content:// URIs via ContentResolver
                    try
                    {
                        var uri = Android.Net.Uri.Parse(_filePathOrUri);
                        var context = Android.App.Application.Context;
                        using var stream = context.ContentResolver.OpenInputStream(uri);
                        if (stream != null)
                        {
                            using var reader = new StreamReader(stream);
                            text = await reader.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception readEx)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            SummaryLabel.Text = $"Error reading content URI: {readEx.Message}";
                            CsvText.Text = string.Empty;
                        });
                        return;
                    }
                }
#endif

                // 3) Fallback: try to interpret as a file path after stripping file:// prefix
                if (text == null && (_filePathOrUri.StartsWith("file://", StringComparison.OrdinalIgnoreCase)))
                {
                    var localPath = _filePathOrUri.Replace("file://", "");
                    if (File.Exists(localPath))
                    {
                        text = await File.ReadAllTextAsync(localPath).ConfigureAwait(false);
                    }
                }

                // Now update UI
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        SummaryLabel.Text = "Report file not found or empty.";
                        CsvText.Text = string.Empty;
                        return;
                    }

                    CsvText.Text = text;

                    var lines = SplitLines(text);
                    if (lines.Count == 0)
                    {
                        SummaryLabel.Text = "No rows found.";
                        return;
                    }

                    // Timestamp of when the report was loaded
                    DateLabel.Text = $"Generated: {DateTime.Now:G}";

                    // Parse CSV into rows (header + data)
                    var rows = new List<string[]>();
                    foreach (var line in lines)
                    {
                        var fields = ParseCsvLine(line);
                        rows.Add(fields);
                    }

                    // Build grid
                    BuildReportGrid(rows);

                    SummaryLabel.Text = $"Rows: {Math.Max(0, rows.Count - 1)}  Columns: {(rows.Count > 0 ? rows[0].Length : 0)}";
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    SummaryLabel.Text = $"Error loading report: {ex.Message}";
                    CsvText.Text = string.Empty;
                });
            }
        }

        void BuildReportGrid(List<string[]> rows)
        {
            ReportGrid.Children.Clear();
            ReportGrid.RowDefinitions.Clear();
            ReportGrid.ColumnDefinitions.Clear();

            if (rows.Count == 0)
                return;

            var header = rows[0];
            int colCount = header.Length;

            // Define columns (equal width)
            for (int c = 0; c < colCount; c++)
            {
                ReportGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // Header row
            ReportGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int c = 0; c < colCount; c++)
            {
                var lbl = new Label
                {
                    Text = header[c],
                    FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Color.FromArgb("#F3F4F6"),
                    Padding = new Thickness(6, 4),
                    LineBreakMode = LineBreakMode.TailTruncation
                };
                ReportGrid.Add(lbl, c, 0);
            }

            // Data rows
            for (int r = 1; r < rows.Count; r++)
            {
                var row = rows[r];
                ReportGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int c = 0; c < colCount; c++)
                {
                    string cellText = c < row.Length ? row[c] : string.Empty;
                    var lbl = new Label
                    {
                        Text = cellText,
                        Padding = new Thickness(6, 4),
                        LineBreakMode = LineBreakMode.TailTruncation
                    };

                    // Alternate row background for readability
                    if ((r % 2) == 0)
                        lbl.BackgroundColor = Color.FromArgb("#FFFFFF");
                    else
                        lbl.BackgroundColor = Color.FromArgb("#FBFBFB");

                    ReportGrid.Add(lbl, c, r);
                }
            }
        }

        static List<string> SplitLines(string text)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
            return lines;
        }

        // Minimal CSV parser handling quoted fields and commas inside quotes.
        static string[] ParseCsvLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return Array.Empty<string>();

            var fields = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                var ch = line[i];

                if (ch == '"')
                {
                    // If already in quotes and next is also quote, treat as escaped quote
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++; // skip the escaped quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (ch == ',' && !inQuotes)
                {
                    fields.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else
                {
                    sb.Append(ch);
                }
            }

            fields.Add(sb.ToString().Trim());
            return fields.ToArray();
        }

        private async void ShareClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_filePathOrUri))
                {
                    await DisplayAlert("Share Error", "Report not available.", "OK");
                    return;
                }

                // If it's a real file path use MAUI Share
                if (File.Exists(_filePathOrUri))
                {
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = Title,
                        File = new ShareFile(_filePathOrUri)
                    });
                    return;
                }

#if ANDROID
                // If it's a content:// URI, use native intent with granted read permission
                if (_filePathOrUri.StartsWith("content://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = Android.Net.Uri.Parse(_filePathOrUri);
                    var intent = new Android.Content.Intent(Android.Content.Intent.ActionSend);
                    intent.SetType("text/csv");
                    intent.PutExtra(Android.Content.Intent.ExtraStream, uri);
                    intent.AddFlags(Android.Content.ActivityFlags.GrantReadUriPermission);

                    var chooser = Android.Content.Intent.CreateChooser(intent, Title);
                    chooser.SetFlags(Android.Content.ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(chooser);
                    return;
                }
#endif

                // Last-chance: try Share with file:// path converted
                if (_filePathOrUri.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    var local = _filePathOrUri.Replace("file://", "");
                    if (File.Exists(local))
                    {
                        await Share.RequestAsync(new ShareFileRequest
                        {
                            Title = Title,
                            File = new ShareFile(local)
                        });
                        return;
                    }
                }

                await DisplayAlert("Share Error", "Cannot share report from provided path/URI.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Share Error", ex.Message, "OK");
            }
        }

        private void ToggleRawClicked(object sender, EventArgs e)
        {
            RawFrame.IsVisible = !RawFrame.IsVisible;
            ToggleRawButton.Text = RawFrame.IsVisible ? "Hide Raw CSV" : "Show Raw CSV";
        }
    }
}