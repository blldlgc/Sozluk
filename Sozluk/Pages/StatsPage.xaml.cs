using Microsoft.Maui.Platform;
using Sozluk.Database;

namespace Sozluk.Pages;

public partial class StatsPage : ContentPage
{

    private readonly QuizHelper _quizHelper;
    public StatsPage()
	{
		InitializeComponent();
        var databaseService = new LocalDatabaseService();
        _quizHelper = new QuizHelper(databaseService.GetConnection());
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatsAsync();
    }


    private async Task LoadStatsAsync()
    {
        var totalStats = await _quizHelper.GetTotalStatsAsync();
        totalLevel1CountLabel.Text = totalStats.Level1Count.ToString();
        totalLevel2CountLabel.Text = totalStats.Level2Count.ToString();
        totalLevel3CountLabel.Text = totalStats.Level3Count.ToString();
        totalLevel4CountLabel.Text = totalStats.Level4Count.ToString();
        totalLevel5CountLabel.Text = totalStats.Level5Count.ToString();
        totalLevel6CountLabel.Text = totalStats.Level6Count.ToString();
        totalLevel7CountLabel.Text = totalStats.Level7Count.ToString();
        var dailyStats = await _quizHelper.GetDailyStatsAsync();
        dailyStatsListView.ItemsSource = dailyStats;
    }
}