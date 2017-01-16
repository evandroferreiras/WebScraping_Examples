module WebScrap
    open FSharp.Data
    let htmlResult = HtmlDocument.Load("https://www.jucerja.rj.gov.br/Servicos/TbPrecos/index.asp")

    let firstTable = htmlResult.Descendants ["table"] |> Seq.tryHead

    let extractRows (table : HtmlNode option) =
       match table with
        | Some t -> t.Descendants ["tr"] 
        | None -> Seq.empty

    let filterRows (row : HtmlNode) =
        match Seq.tryHead(row.Descendants ["td"]) with
            | Some r -> (r.HasAttribute("valign","middle") || r.HasAttribute("colspan","6")) |> not
            | None -> false

    let rows = firstTable |> 
                extractRows |>  
                Seq.filter( fun x ->   filterRows(x))

    let FirstAndSecondCellValues (row : HtmlNode) =
        row.Descendants ["td"] |> Seq.take 2 |> Seq.map (fun x -> x.InnerText()  ) |> Seq.toArray

    let result = rows |> Seq.map (fun x -> FirstAndSecondCellValues(x) ) 
