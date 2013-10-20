using System;
using System.ServiceModel;

namespace RemoteConsole
{
	[ServiceContract]
	public interface IConsole
	{
		[OperationContract]
		void Write(object o);
		[OperationContract]
		void WriteLine();
		[OperationContract]
		void WriteLine(string format, params object[] args);
		[OperationContract]
		void WriteLine(object o);
		[OperationContract]
		void SetCursorPosition(int x, int y);
		int CursorTop { [OperationContract] get;}
		int BufferHeight { [OperationContract] get;}
		[OperationContract]
		ConsoleKeyInfo ReadKey(bool intercept);		
		int WindowWidth { [OperationContract] get;}
		[OperationContract]
		void Clear();
		[OperationContract]
		string ReadLine();
	}

	[ServiceContract]
	public interface IConsoleCallback{
		[OperationContract]
		void CancelKeyPress(ConsoleCancelEventArgs args);
	}
}

