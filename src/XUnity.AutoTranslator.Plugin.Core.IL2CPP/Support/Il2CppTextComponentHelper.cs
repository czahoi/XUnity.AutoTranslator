﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using XUnity.AutoTranslator.Plugin.Core.Configuration;
using XUnity.AutoTranslator.Plugin.Core.Extensions;
using XUnity.AutoTranslator.Plugin.Core.IL2CPP.Text;
using XUnity.Common.Constants;
using XUnity.Common.Utilities;

namespace XUnity.AutoTranslator.Plugin.Core.Support
{
   internal class Il2CppTextComponentHelper : ITextComponentHelper
   {
      private static GameObject[] _objects = new GameObject[ 128 ];
      private static readonly string XuaIgnore = "XUAIGNORE";

      public string GetText( object ui )
      {
         return ( ui as ITextComponent )?.text;
      }

      public bool IsComponentActive( object ui )
      {
         return ( ( ui as ITextComponent )?.Component ).gameObject?.activeInHierarchy ?? false;
      }

      public bool IsKnownTextType( object ui )
      {
         return ui is ITextComponent;
      }

      public bool IsNGUI( object ui )
      {
         return false;
      }

      public bool IsSpammingComponent( object ui )
      {
         return ui is ITextComponent tc && tc.IsSpammingComponent();
      }

      public void SetText( object ui, string text )
      {
         if( ui is ITextComponent tc )
         {
            tc.text = text;
         }
      }

      public bool ShouldTranslateTextComponent( object ui, bool ignoreComponentState )
      {
         if( ui is ITextComponent tc && tc.Component != null )
         {
            var component = tc.Component;

            // dummy check
            var go = component.gameObject;
            var ignore = go.HasIgnoredName();
            if( ignore )
            {
               return false;
            }

            if( !ignoreComponentState )
            {
               var behaviour = component.TryCast<Behaviour>();
               if( !go.activeInHierarchy || behaviour?.enabled == false ) // legacy "isActiveAndEnabled"
               {
                  return false;
               }
            }

            return !tc.IsPlaceholder();
         }

         return true;
      }

      public bool SupportsLineParser( object ui )
      {
         return Settings.GameLogTextPaths.Count > 0 && Settings.GameLogTextPaths.Contains( ( ( ui as ITextComponent )?.Component ).gameObject.GetPath() );
      }

      public bool SupportsRichText( object ui )
      {
         return ui is ITextComponent tc && tc.SupportsRichText();
      }

      public bool SupportsStabilization( object ui )
      {
         if( ui == null ) return false;

         return true;
      }

      public TextTranslationInfo GetOrCreateTextTranslationInfo( object ui )
      {
         var info = ui.GetOrCreateExtensionData<Il2CppTextTranslationInfo>();
         info.Initialize( ui );

         return info;
      }

      public TextTranslationInfo GetTextTranslationInfo( object ui )
      {
         var info = ui.GetExtensionData<Il2CppTextTranslationInfo>();

         return info;
      }

      public object CreateWrapperTextComponentIfRequiredAndPossible( object ui )
      {
         if( ui is Component tc )
         {
            var type = tc.GetIl2CppType();

            if( Settings.EnableUGUI && UnityTypes.Text != null && UnityTypes.Text.Il2CppType.IsAssignableFrom( type ) )
            {
               return new TextComponent( tc );
            }
            else if( Settings.EnableTextMesh && UnityTypes.TextMesh != null && UnityTypes.TextMesh.Il2CppType.IsAssignableFrom( type ) )
            {
               return new TextMeshComponent( tc );
            }
            else if( Settings.EnableTextMeshPro && UnityTypes.TMP_Text != null && UnityTypes.TMP_Text.Il2CppType.IsAssignableFrom( type ) )
            {
               return new TMP_TextComponent( tc );
            }
         }
         return null;
      }

      public IEnumerable<object> GetAllTextComponentsInChildren( object go )
      {
         yield break;
      }

      public string[] GetPathSegments( object obj )
      {
         if( obj is GameObject go )
         {

         }
         else if( obj is ITextComponent tc )
         {
            go = tc.Component.gameObject;
         }
         else if( obj is Component comp )
         {
            go = comp.gameObject;
         }
         else
         {
            throw new ArgumentException( "Expected object to be a GameObject or component.", "obj" );
         }

         int i = 0;
         int j = 0;

         _objects[ i++ ] = go;
         while( go.transform.parent != null )
         {
            go = go.transform.parent.gameObject;
            _objects[ i++ ] = go;
         }

         var result = new string[ i ];
         while( --i >= 0 )
         {
            result[ j++ ] = _objects[ i ].name;
            _objects[ i ] = null;
         }

         return result;
      }

      public string GetPath( object obj )
      {
         if( obj is GameObject go )
         {

         }
         else if( obj is ITextComponent tc )
         {
            go = tc.Component.gameObject;
         }
         else if( obj is Component comp )
         {
            go = comp.gameObject;
         }
         else
         {
            throw new ArgumentException( "Expected object to be a GameObject or component.", "obj" );
         }

         StringBuilder path = new StringBuilder();
         var segments = GetPathSegments( go );
         for( int i = 0; i < segments.Length; i++ )
         {
            path.Append( "/" ).Append( segments[ i ] );
         }

         return path.ToString();
      }

      public bool HasIgnoredName( object obj )
      {
         if( obj is GameObject go )
         {

         }
         else if( obj is ITextComponent tc )
         {
            go = tc.Component.gameObject;
         }
         else if( obj is Component comp )
         {
            go = comp.gameObject;
         }
         else
         {
            throw new ArgumentException( "Expected object to be a GameObject or component.", "obj" );
         }

         return go.name.Contains( XuaIgnore );
      }

      public string GetTextureName( object texture, string fallbackName )
      {
         if( texture is Texture2D texture2d )
         {
            var name = texture2d.name;
            if( !string.IsNullOrEmpty( name ) )
            {
               return name;
            }
         }
         return fallbackName;
      }

      public void LoadImageEx( object texture, byte[] data, object originalTexture )
      {
         // why no Image Conversion?
         throw new NotImplementedException();
      }

      public TextureDataResult GetTextureData( object texture )
      {
         // why no Image Conversion?
         throw new NotImplementedException();
      }

      public bool IsKnownImageType( object ui )
      {
         return false;
      }

      public object GetTexture( object ui )
      {
         return null;
      }

      public void SetTexture( object ui, object texture )
      {
         
      }

      public void SetAllDirtyEx( object ui )
      {
      }

      public object CreateEmptyTexture2D()
      {
         return new Texture2D( 2, 2, TextureFormat.ARGB32, false );
      }
   }
}