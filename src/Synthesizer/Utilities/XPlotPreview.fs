namespace Synthesizer

module previewarr =
    open XPlot.Plotly
    let chart title (array:List<float>) =
        array |> Chart.Line |> Chart.WithOptions(Options(title = title)) |> Chart.Show