<!-- default file list -->
*Files to look at*:

* [CustomException.cs](./CS/T124512/App_Code/CustomException.cs) (VB: [CustomException.vb](./VB/T124512/App_Code/CustomException.vb))
* [WorldCities.cs](./CS/T124512/App_Code/WorldCities.cs) (VB: [WorldCities.vb](./VB/T124512/App_Code/WorldCities.vb))
* [BatchEditScript.js](./CS/T124512/BatchEditScript.js) (VB: [BatchEditScript.js](./VB/T124512/BatchEditScript.js))
* **[Default.aspx](./CS/T124512/Default.aspx) (VB: [Default.aspx](./VB/T124512/Default.aspx))**
* [Default.aspx.cs](./CS/T124512/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/T124512/Default.aspx.vb))
<!-- default file list end -->
# ASPxGridView - How to implement cascading comboboxes in Batch Edit mode


<p>In this example, the combo box in the City column (the City combo box) is populated dynamically with city names via callbacks, based on the value selected in the combo box in the Country column (the Country combo box).  <br>You can find detailed steps by clicking below the "Show Implementation Details" link .<br><br><strong>MVC:</strong><br><a href="https://www.devexpress.com/Support/Center/p/T155879">GridView - A simple implementation of cascading comboboxes in Batch Edit mode</a><br><br></p>
<p><strong>See also: </strong><br><a href="https://www.devexpress.com/Support/Center/p/T356740">ASPxGridView - How to implement cascading combo boxes in Batch Edit mode by using WebMethods</a></p>


<h3>Description</h3>

<p><strong>In v16.1</strong>, we supported the callback mode for&nbsp;GridViewComboBoxColumn and the EditItemTemplate implementation is not necessary.&nbsp;The main steps are:<br>1) Use the client-side&nbsp;<a href="https://documentation.devexpress.com/AspNet/DevExpressWebASPxGridViewScriptsASPxClientGridView_BatchEditStartEditingtopic.aspx">ASPxClientGridView.BatchEditStartEditing</a>&nbsp;event to check the main combo box value and update the child combo box data (if it's necessary).</p>
<p>2)&nbsp;Handle the&nbsp;<a href="https://documentation.devexpress.com/AspNet/DevExpressWebASPxEditorsScriptsASPxClientComboBox_SelectedIndexChangedtopic.aspx">SelectedIndexChanged</a>&nbsp;event to send callbacks when a user changes a value in the main combo box.<br>3)&nbsp;Use&nbsp;<a href="https://documentation.devexpress.com/#AspNet/clsDevExpressWebASPxHiddenFieldtopic">an ASPxHiddenField</a>&nbsp;to pass information about the last selected value in the main editor to the server. This step is necessary to update data on a custom callback and to be able to get the last selected value when filtering is performed.<br>4)&nbsp;Handle the&nbsp;<a href="https://documentation.devexpress.com/AspNet/DevExpressWebASPxEditorsScriptsASPxClientComboBox_EndCallbacktopic.aspx">ASPxClientComboBox.EndCallback</a>&nbsp;event for the second editor to select an item after a custom callback.</p>

<br/>


