namespace Synthesizer

module Previewarr =
    open XPlot.Plotly
    let Chart Title (Array:List<float>) =
        Array |> Chart.Line |> Chart.WithOptions(Options(Title = Title)) |> Chart.Show
    
    let ChartXY Title (x:List<float>) (y:List<float>) =
        Scatter(x=x, y=y) |> Chart.Plot |> Chart.WithLayout(Layout(Title=Title)) |> Chart.Show