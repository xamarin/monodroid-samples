
/*
 * Copyright (C) 2016 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Lang;

namespace AppShortcuts
{
	public class Utils
	{
		private Utils() {}

		public static void ShowToast(Context context, string message)
		{
			new Handler(Looper.MainLooper).Post(new HandlerRunnable(context, message));
		}

		private class HandlerRunnable : Java.Lang.Object, IRunnable
		{
			Context Context { get; set; }
			string Message { get; set; }

			public HandlerRunnable(Context context, string message)
			{
				Context = context;
				Message = message;
			}
			public void Run()
			{
				Toast.MakeText(Context, Message, ToastLength.Short).Show();
			}
		}
	}
}