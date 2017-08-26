# dotnetconf-local-events-formatter

Simple console app to format dotnetconf local event data from surveymonkey dump to an HTML table. I didn't write it so if the VB.NET is wonky I'd really have no idea.

Requires a secrets.config in the LocalEventsFormatter directory with Bing MAps key:

```xml
<appSettings>
  <add key="BingKey" value="yoursecretkeygoeshere" />
</appSettings>
```
