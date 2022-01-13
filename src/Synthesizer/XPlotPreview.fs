namespace Synthesizer

module previewarr =
    open XPlot.Plotly
    let chart (array:List<float>) =
        array |> Chart.Line |> Chart.WithOptions(Options(title = "sinusoïdal")) |> Chart.Show