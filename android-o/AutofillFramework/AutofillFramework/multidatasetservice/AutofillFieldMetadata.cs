﻿using System;
using Android.Service.Autofill;
using Android.Text;
using Android.Views;
using Android.Views.Autofill;
using static Android.App.Assist.AssistStructure;

namespace AutofillFramework
{
	/**
	 * A stripped down version of a {@link ViewNode} that contains only autofill-relevant metadata. It
	 * also contains a {@code mSaveType} flag that is calculated based on the {@link ViewNode}]'s
	 * autofill hints.
	 */
	public class AutofillFieldMetadata
	{
		public SaveDataType SaveType { get; set; }

		public string[] AutofillHints {
			get => AutofillHints;
			set
			{
				AutofillHints = value;
				UpdateSaveTypeFromHints();
			} 
		}

		public AutofillId AutofillId { get; }
		public int AutofillType { get; }
		string[] AutofillOptions { get; }
		public bool Focused { get; }

		public AutofillFieldMetadata(ViewNode view)
		{
			AutofillId = view.AutofillId;
			AutofillType = (int)view.AutofillType;
			AutofillOptions = view.GetAutofillOptions();
			Focused = view.IsFocused;
			AutofillHints = AutofillHelper.FilterForSupportedHints(view.GetAutofillHints());
		}

		/**
	     * When the {@link ViewNode} is a list that the user needs to choose a string from (i.e. a
	     * spinner), this is called to return the index of a specific item in the list.
	     */
		public int GetAutofillOptionIndex(String value)
		{
			for (int i = 0; i < AutofillOptions.Length; i++)
			{
				if (AutofillOptions[i].Equals(value))
				{
					return i;
				}
			}
			return -1;
		}

		void UpdateSaveTypeFromHints()
		{
			SaveType = 0;
			if (AutofillHints == null)
			{
				return;
			}
			foreach (var hint in AutofillHints)
			{
				switch (hint)
				{
					case View.AutofillHintCreditCardExpirationDate:
					case View.AutofillHintCreditCardExpirationDay:
					case View.AutofillHintCreditCardExpirationMonth:
					case View.AutofillHintCreditCardExpirationYear:
					case View.AutofillHintCreditCardNumber:
					case View.AutofillHintCreditCardSecurityCode:
						SaveType |= SaveDataType.CreditCard;
						break;
					case View.AutofillHintEmailAddress:
						SaveType |= SaveDataType.EmailAddress;
						break;
					case View.AutofillHintPhone:
					case View.AutofillHintName:
						SaveType |= SaveDataType.Generic;
						break;
					case View.AutofillHintPassword:
						SaveType |= SaveDataType.Password;
						SaveType &= ~SaveDataType.EmailAddress;
						SaveType &= ~SaveDataType.Username;
						break;
					case View.AutofillHintPostalAddress:
					case View.AutofillHintPostalCode:
						SaveType |= SaveDataType.Address;
						break;
					case View.AutofillHintUsername:
						SaveType |= SaveDataType.Username;
						break;
				}
			}
		}
	}
}