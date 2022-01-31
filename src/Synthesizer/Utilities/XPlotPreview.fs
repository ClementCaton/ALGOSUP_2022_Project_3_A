namespace Synthesizer

module previewarr =
    open XPlot.Plotly
    let chart title (array:List<float>) =
        array |> Chart.Line |> Chart.WithOptions(Options(title = title)) |> Chart.Show
    
    let chartXY title (x:List<float>) (y:List<float>) =
        Scatter(x=x, y=y) |> Chart.Plot |> Chart.WithLayout(Layout(title=title)) |> Chart.Show