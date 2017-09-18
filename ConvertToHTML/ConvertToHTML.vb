Imports BingMapsRESTToolkit

Public Class ConvertToHTML

    Public BingMapsKey As String

    Public Sub Convert(theList As List(Of MyEvent), outputfile As String)

        Dim html = <table>
                       <tbody id="launch-data">
                           <%= From i In theList
                               Order By CDate(i.TheDate), i.City Ascending
                               Select
                                GetRow(i) %>
                       </tbody>
                   </table>


        If outputfile = "" OrElse outputfile Is Nothing Then
            outputfile = Environment.CurrentDirectory + "\output.html"
        End If

        html.Save(outputfile, SaveOptions.DisableFormatting)
    End Sub

    Function GetRow(i As MyEvent) As XElement
        Dim returnVal = <tr></tr>

        'Try
        Dim dateElement = <td><%= CDate(i.TheDate).ToString("MMMM d") %></td>
        Dim cityElement = <td><%= i.City %></td>
        Dim urlElement = GetUrl(i.Url, CDate(i.TheDate))

        Dim loc As New DataLib.Location() With {.City = i.City}
        GetGeoCode(loc)

        returnVal = <tr>
                        <%= dateElement %>
                        <%= cityElement %>
                        <%= urlElement %>
                        <td style="display:none"><%= loc.Lat %></td>
                        <td style="display:none"><%= loc.Long %></td>
                    </tr>

        'Catch ex As Exception
        'End Try
        Return returnVal
    End Function

    Function GetUrl(UrlString As String, theDate As Date) As XElement
        Dim returnVal As XElement = <td>TBA</td>

        If UrlString IsNot Nothing Then
            If UrlString.Contains("http") Then
                If theDate >= Date.Today Then
                    returnVal = <td><a href=<%= UrlString %>>Register Here</a></td>
                Else
                    returnVal = <td><a href=<%= UrlString %>>Completed</a></td>
                End If
            End If
        End If
        Return returnVal
    End Function

    Sub GetGeoCode(loc As DataLib.Location)

        'Return lat & long
        Dim search = DataLib.LocationsService.ReadLocation(loc.City)
        If search IsNot Nothing Then
            loc.Lat = search.Lat
            loc.Long = search.Long
        Else

            Dim request As New GeocodeRequest With {
            .Query = loc.City,
            .MaxResults = 1,
            .BingMapsKey = BingMapsKey}

            Dim response = ServiceManager.GetResponseAsync(request).GetAwaiter().GetResult()
            If response?.ResourceSets(0)?.Resources?.Length > 0 Then

                Dim result = CType(response.ResourceSets(0).Resources(0), BingMapsRESTToolkit.Location)

                loc.Lat = result.Point.Coordinates(0)
                loc.Long = result.Point.Coordinates(1)

                DataLib.LocationsService.WriteLocation(loc)

            End If
        End If

    End Sub


End Class



Public Class MyEvent
    Public Property TheDate() As String
    Public Property City() As String
    Public Property Url() As String
End Class
