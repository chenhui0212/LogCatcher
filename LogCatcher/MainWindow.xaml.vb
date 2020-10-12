Imports Renci.SshNet
Imports Renci.SshNet.Common

Class MainWindow
    Dim client As SshClient


    Private Sub Button_Click_Connect(sender As Object, e As RoutedEventArgs)
        Dim env As String = TxtEnv.Text
        Dim port As Integer = TxtPort.Text
        Dim username As String = "HLL"
        Dim password As String = "HLL"
        Dim server As String
        If env Like "TEST" Then
            server = "172.xxx.0.1"
        ElseIf env Like "UAT" Then
            server = "172.xxx.0.2"
        End If

        Dim connectionInfo = New PasswordConnectionInfo(server, port, username, password)
        connectionInfo.Timeout = TimeSpan.FromSeconds(5)

        If BtnConn.Content Like "连接" Then
            Connect(connectionInfo)
        ElseIf BtnConn.Content Like "断开" Then
            Disconnect()
        End If
    End Sub


    Private Sub Button_Click_LogCatch(sender As Object, e As RoutedEventArgs)
        Dim env As String = TxtEnv.Text
        Dim trcID As String = TxtTrcID.Text
        Dim keyword As String = TxtKeyWord.Text
        Dim logName As String
        If env Like "TEST" Then
            logName = "test-grpc.log"
        ElseIf env Like "UAT" Then
            logName = "uat-grpc.log"
        End If

        Dim cmdStr As String = "grep '" + trcID + "' /home/logs/grpc-server-launcher/" + logName + " | grep '" + keyword + "'"
        ExecCommand(cmdStr)
    End Sub

    Private Sub Button_Click_LogClear(sender As Object, e As RoutedEventArgs)
        TxtShow.Document.Blocks.Clear()
    End Sub


    Private Sub Connect(connectionInfo As [PasswordConnectionInfo])
        Try
            ' Create a SshClient
            TxtShow.AppendText("连接中..." & Environment.NewLine)
            client = New SshClient(connectionInfo)
            client.Connect()

            BtnConn.Content = "断开"
            TxtShow.AppendText("连接成功!" & Environment.NewLine)
        Catch e As ArgumentException
            TxtShow.AppendText("连接失败!" & e.Message & Environment.NewLine)
            Debug.WriteLine("Host or Username is invalid: {0}", e)
        Catch e As SshException
            TxtShow.AppendText("连接失败..." & e.Message & Environment.NewLine)
            Debug.WriteLine("SshException: {0}", e)
        End Try
    End Sub


    Private Sub ExecCommand(cmdStr As String)
        Try
            ' Execute command
            Dim cmd As SshCommand = client.CreateCommand(cmdStr)
            cmd.Execute()

            TxtShow.AppendText("执行结果：" & cmd.Result & Environment.NewLine)
        Catch e As ArgumentNullException
            Debug.WriteLine("ArgumentNullException: {0}", e)
        End Try
    End Sub


    Private Sub Disconnect()
        Try
            TxtShow.AppendText("断开中..." & Environment.NewLine)
            client.Disconnect()

            BtnConn.Content = "连接"
            TxtShow.AppendText("已断开!" & Environment.NewLine)
        Catch e As ObjectDisposedException
            Debug.WriteLine("ObjectDisposedException: {0}", e)
        End Try
    End Sub

End Class
