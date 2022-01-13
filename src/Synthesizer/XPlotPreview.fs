namespace Synthesizer

module previewarr =
    open XPlot.Plotly
    let chart (array:float[]) =
        array |> Chart.Line |> Chart.WithOptions(Options(title = "sinusoïdal")) |> Chart.Show