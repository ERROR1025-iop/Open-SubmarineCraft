package com.unity3d.player;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.webkit.WebView;
 
public class PrivacyActivity extends Activity implements DialogInterface.OnClickListener {

   // 隐私协议内容
   final String privacyContext =
             "请你务必审慎阅读、充分理解“隐私政策”个条款，包括但不限于：为了向你提供内容等服务，我们需要收集你的设备信息、操作日志等个人信息。你可以在“设置”中查看、变更、删除个人信息并管理你的授权。你可以阅读<a href=\"https://sdk.aipie.cool/policy/user_agreement?n=方块潜艇\">《用户协议》</a>和<a href=\"https://sdk.aipie.cool/policy/privacy_agreement?n=方块潜艇\">《用户隐私政策》</a>了解详细信息。如你同意，请点击“同意”开始接受我们的服务。";
     
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
  
        // 如果已经同意过隐私协议则直接进入Unity Activity
        if (GetPrivacyAccept()){
            EnterUnityActivity();
            return;
        }
        // 弹出隐私协议对话框
        ShowPrivacyDialog();
    }
 
    // 显示隐私协议对话框
    private void ShowPrivacyDialog(){
        WebView webView = new WebView(this);
        webView.loadData(privacyContext, "text/html", "utf-8");         
        AlertDialog.Builder privacyDialog = new AlertDialog.Builder(this);
        privacyDialog.setCancelable(false);
        privacyDialog.setView(webView);
        privacyDialog.setTitle("提示");
        privacyDialog.setNegativeButton("拒绝",this);
        privacyDialog.setPositiveButton("同意",this);
        privacyDialog.create().show();
    }
    
    @Override
    public void onClick(DialogInterface dialogInterface, int i) {
        switch (i){
            case AlertDialog.BUTTON_POSITIVE://点击同意按钮
                SetPrivacyAccept(true);
                EnterUnityActivity(); //启动Unity Activity
                break;
            case AlertDialog.BUTTON_NEGATIVE://点击拒绝按钮,直接退出App
                finish();
                break;
        }
    }
    
    // 启动Unity Activity
    private void EnterUnityActivity(){
        Intent unityAct = new Intent();
        unityAct.setClassName(this, "com.unity3d.player.UnityPlayerActivity");
        this.startActivity(unityAct);
    }
    
    // 本地存储保存同意隐私协议状态
    private void SetPrivacyAccept(boolean accepted){
        SharedPreferences.Editor prefs = this.getSharedPreferences("PlayerPrefs", MODE_PRIVATE).edit();
        prefs.putBoolean("PrivacyAcceptedKey", accepted);
        prefs.apply();
    }
    
    // 获取是否已经同意过
    private boolean GetPrivacyAccept(){
        SharedPreferences prefs = this.getSharedPreferences("PlayerPrefs", MODE_PRIVATE);
        return prefs.getBoolean("PrivacyAcceptedKey", false);
    }
}
