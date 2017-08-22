﻿using System;
using Android.App;
using Android.App.Assist;
using Android.OS;
using Android.Service.Autofill;
using Android.Util;
using AutofillFramework.multidatasetservice.datasource;
using AutofillFramework.multidatasetservice.settings;
using Java.Util;

namespace AutofillFramework
{
	[Service(Label = "Multi-Dataset Autofill Service", Permission = "android.permission.BIND_AUTOFILL")]
	[IntentFilter(new[]{"android.service.autofill.AutofillService"})]
	[MetaData("android.autofill", Resource = "@xml/multidataset_service")]
	public class MyAutofillService : AutofillService
	{
		public override void OnFillRequest(FillRequest request, CancellationSignal cancellationSignal, FillCallback callback)
		{
			var structure = request.FillContexts[request.FillContexts.Count - 1].Structure;
			var data = request.ClientState;
			Log.Debug(CommonUtil.Tag, "onFillRequest(): data=" + CommonUtil.BundleToString(data));

			cancellationSignal.CancelEvent += (sender, e) => {
				Log.Warn(CommonUtil.Tag, "Cancel autofill not implemented in this sample.");	
			};
			// Parse AutoFill data in Activity
			var parser = new StructureParser(structure);
			parser.ParseForFill();
			AutofillFieldMetadataCollection autofillFields = parser.AutofillFields;
			var responseBuilder = new FillResponse.Builder();
			// Check user's settings for authenticating Responses and Datasets.
			bool responseAuth = MyPreferences.GetInstance(this).IsResponseAuth();
			var autofillIds = autofillFields.GetAutofillIds();
			if (responseAuth && !(Arrays.AsList(autofillIds).Count == 0))
			{
				// If the entire Autofill Response is authenticated, AuthActivity is used
				// to generate Response.
				var sender = AuthActivity.GetAuthIntentSenderForResponse(this);
				var presentation = AutofillHelper
					.NewRemoteViews(PackageName, GetString(Resource.String.autofill_sign_in_prompt),
								Resource.Drawable.ic_lock_black_24dp);
				responseBuilder
						.SetAuthentication(autofillIds, sender, presentation);
				callback.OnSuccess(responseBuilder.Build());
			}
			else
			{
				var datasetAuth = MyPreferences.GetInstance(this).IsDatasetAuth();
				var clientFormDataMap = SharedPrefsAutofillRepository.GetInstance(this).GetFilledAutofillFieldCollection
				                                                     (autofillFields.FocusedAutofillHints, autofillFields.AllAutofillHints);
				var response = AutofillHelper.NewResponse(this, datasetAuth, autofillFields, clientFormDataMap);
				callback.OnSuccess(response);
			}
		}

		public override void OnSaveRequest(SaveRequest request, SaveCallback callback)
		{
			var context = request.FillContexts;
			var structure = context[context.Count - 1].Structure;
			var data = request.ClientState;
			Log.Debug(CommonUtil.Tag, "onSaveRequest(): data=" + CommonUtil.BundleToString(data));
			var parser = new StructureParser(structure);
			parser.ParseForSave();
			var filledAutofillFieldCollection = parser.ClientFormData;
			SharedPrefsAutofillRepository.GetInstance(this).SaveFilledAutofillFieldCollection(filledAutofillFieldCollection);
		}

		public override void OnConnected()
		{
			Log.Debug(CommonUtil.Tag, "onConnected");
		}

		public override void OnDisconnected()
		{
			Log.Debug(CommonUtil.Tag, "onDisconnected");
		}
	}
}