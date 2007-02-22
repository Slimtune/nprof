using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NProf.GUI
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel linkLabel1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(392, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "nprof is Copyright 2002-2004 by Matthew Mastracci.  All Rights Reserved.";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(8, 40);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(640, 240);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "\t\t    GNU GENERAL PUBLIC LICENSE\n\t\t       Version 2, June 1991\n\n Copyright (C) 19" +
				"89, 1991 Free Software Foundation, Inc.\n                       59 Temple Place, " +
				"Suite 330, Boston, MA  02111-1307  USA\n Everyone is permitted to copy and distri" +
				"bute verbatim copies\n of this license document, but changing it is not allowed.\n" +
				"\n\t\t\t    Preamble\n\n  The licenses for most software are designed to take away you" +
				"r\nfreedom to share and change it.  By contrast, the GNU General Public\nLicense i" +
				"s intended to guarantee your freedom to share and change free\nsoftware--to make " +
				"sure the software is free for all its users.  This\nGeneral Public License applie" +
				"s to most of the Free Software\nFoundation\'s software and to any other program wh" +
				"ose authors commit to\nusing it.  (Some other Free Software Foundation software i" +
				"s covered by\nthe GNU Library General Public License instead.)  You can apply it " +
				"to\nyour programs, too.\n\n  When we speak of free software, we are referring to fr" +
				"eedom, not\nprice.  Our General Public Licenses are designed to make sure that yo" +
				"u\nhave the freedom to distribute copies of free software (and charge for\nthis se" +
				"rvice if you wish), that you receive source code or can get it\nif you want it, t" +
				"hat you can change the software or use pieces of it\nin new free programs; and th" +
				"at you know you can do these things.\n\n  To protect your rights, we need to make " +
				"restrictions that forbid\nanyone to deny you these rights or to ask you to surren" +
				"der the rights.\nThese restrictions translate to certain responsibilities for you" +
				" if you\ndistribute copies of the software, or if you modify it.\n\n  For example, " +
				"if you distribute copies of such a program, whether\ngratis or for a fee, you mus" +
				"t give the recipients all the rights that\nyou have.  You must make sure that the" +
				"y, too, receive or can get the\nsource code.  And you must show them these terms " +
				"so they know their\nrights.\n\n  We protect your rights with two steps: (1) copyrig" +
				"ht the software, and\n(2) offer you this license which gives you legal permission" +
				" to copy,\ndistribute and/or modify the software.\n\n  Also, for each author\'s prot" +
				"ection and ours, we want to make certain\nthat everyone understands that there is" +
				" no warranty for this free\nsoftware.  If the software is modified by someone els" +
				"e and passed on, we\nwant its recipients to know that what they have is not the o" +
				"riginal, so\nthat any problems introduced by others will not reflect on the origi" +
				"nal\nauthors\' reputations.\n\n  Finally, any free program is threatened constantly " +
				"by software\npatents.  We wish to avoid the danger that redistributors of a free\n" +
				"program will individually obtain patent licenses, in effect making the\nprogram p" +
				"roprietary.  To prevent this, we have made it clear that any\npatent must be lice" +
				"nsed for everyone\'s free use or not licensed at all.\n\n  The precise terms and co" +
				"nditions for copying, distribution and\nmodification follow.\n\n\t\t    GNU GENERAL " +
				"PUBLIC LICENSE\n   TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATIO" +
				"N\n\n  0. This License applies to any program or other work which contains\na notic" +
				"e placed by the copyright holder saying it may be distributed\nunder the terms of" +
				" this General Public License.  The \"Program\", below,\nrefers to any such program " +
				"or work, and a \"work based on the Program\"\nmeans either the Program or any deriv" +
				"ative work under copyright law:\nthat is to say, a work containing the Program or" +
				" a portion of it,\neither verbatim or with modifications and/or translated into a" +
				"nother\nlanguage.  (Hereinafter, translation is included without limitation in\nth" +
				"e term \"modification\".)  Each licensee is addressed as \"you\".\n\nActivities other " +
				"than copying, distribution and modification are not\ncovered by this License; the" +
				"y are outside its scope.  The act of\nrunning the Program is not restricted, and " +
				"the output from the Program\nis covered only if its contents constitute a work ba" +
				"sed on the\nProgram (independent of having been made by running the Program).\nWhe" +
				"ther that is true depends on what the Program does.\n\n  1. You may copy and distr" +
				"ibute verbatim copies of the Program\'s\nsource code as you receive it, in any med" +
				"ium, provided that you\nconspicuously and appropriately publish on each copy an a" +
				"ppropriate\ncopyright notice and disclaimer of warranty; keep intact all the\nnoti" +
				"ces that refer to this License and to the absence of any warranty;\nand give any " +
				"other recipients of the Program a copy of this License\nalong with the Program.\n\n" +
				"You may charge a fee for the physical act of transferring a copy, and\nyou may at" +
				" your option offer warranty protection in exchange for a fee.\n\n  2. You may modi" +
				"fy your copy or copies of the Program or any portion\nof it, thus forming a work " +
				"based on the Program, and copy and\ndistribute such modifications or work under t" +
				"he terms of Section 1\nabove, provided that you also meet all of these conditions" +
				":\n\n    a) You must cause the modified files to carry prominent notices\n    stati" +
				"ng that you changed the files and the date of any change.\n\n    b) You must cause" +
				" any work that you distribute or publish, that in\n    whole or in part contains " +
				"or is derived from the Program or any\n    part thereof, to be licensed as a whol" +
				"e at no charge to all third\n    parties under the terms of this License.\n\n    c)" +
				" If the modified program normally reads commands interactively\n    when run, you" +
				" must cause it, when started running for such\n    interactive use in the most or" +
				"dinary way, to print or display an\n    announcement including an appropriate cop" +
				"yright notice and a\n    notice that there is no warranty (or else, saying that y" +
				"ou provide\n    a warranty) and that users may redistribute the program under\n   " +
				" these conditions, and telling the user how to view a copy of this\n    License. " +
				" (Exception: if the Program itself is interactive but\n    does not normally prin" +
				"t such an announcement, your work based on\n    the Program is not required to pr" +
				"int an announcement.)\n\nThese requirements apply to the modified work as a whole" +
				".  If\nidentifiable sections of that work are not derived from the Program,\nand c" +
				"an be reasonably considered independent and separate works in\nthemselves, then t" +
				"his License, and its terms, do not apply to those\nsections when you distribute t" +
				"hem as separate works.  But when you\ndistribute the same sections as part of a w" +
				"hole which is a work based\non the Program, the distribution of the whole must be" +
				" on the terms of\nthis License, whose permissions for other licensees extend to t" +
				"he\nentire whole, and thus to each and every part regardless of who wrote it.\n\nTh" +
				"us, it is not the intent of this section to claim rights or contest\nyour rights " +
				"to work written entirely by you; rather, the intent is to\nexercise the right to " +
				"control the distribution of derivative or\ncollective works based on the Program." +
				"\n\nIn addition, mere aggregation of another work not based on the Program\nwith th" +
				"e Program (or with a work based on the Program) on a volume of\na storage or dist" +
				"ribution medium does not bring the other work under\nthe scope of this License.\n\n" +
				"  3. You may copy and distribute the Program (or a work based on it,\nunder Secti" +
				"on 2) in object code or executable form under the terms of\nSections 1 and 2 abov" +
				"e provided that you also do one of the following:\n\n    a) Accompany it with the " +
				"complete corresponding machine-readable\n    source code, which must be distribut" +
				"ed under the terms of Sections\n    1 and 2 above on a medium customarily used fo" +
				"r software interchange; or,\n\n    b) Accompany it with a written offer, valid for" +
				" at least three\n    years, to give any third party, for a charge no more than yo" +
				"ur\n    cost of physically performing source distribution, a complete\n    machine" +
				"-readable copy of the corresponding source code, to be\n    distributed under the" +
				" terms of Sections 1 and 2 above on a medium\n    customarily used for software i" +
				"nterchange; or,\n\n    c) Accompany it with the information you received as to the" +
				" offer\n    to distribute corresponding source code.  (This alternative is\n    al" +
				"lowed only for noncommercial distribution and only if you\n    received the progr" +
				"am in object code or executable form with such\n    an offer, in accord with Subs" +
				"ection b above.)\n\nThe source code for a work means the preferred form of the wor" +
				"k for\nmaking modifications to it.  For an executable work, complete source\ncode " +
				"means all the source code for all modules it contains, plus any\nassociated inter" +
				"face definition files, plus the scripts used to\ncontrol compilation and installa" +
				"tion of the executable.  However, as a\nspecial exception, the source code distri" +
				"buted need not include\nanything that is normally distributed (in either source o" +
				"r binary\nform) with the major components (compiler, kernel, and so on) of the\nop" +
				"erating system on which the executable runs, unless that component\nitself accomp" +
				"anies the executable.\n\nIf distribution of executable or object code is made by o" +
				"ffering\naccess to copy from a designated place, then offering equivalent\naccess " +
				"to copy the source code from the same place counts as\ndistribution of the source" +
				" code, even though third parties are not\ncompelled to copy the source along with" +
				" the object code.\n\n  4. You may not copy, modify, sublicense, or distribute the" +
				" Program\nexcept as expressly provided under this License.  Any attempt\notherwise" +
				" to copy, modify, sublicense or distribute the Program is\nvoid, and will automat" +
				"ically terminate your rights under this License.\nHowever, parties who have recei" +
				"ved copies, or rights, from you under\nthis License will not have their licenses " +
				"terminated so long as such\nparties remain in full compliance.\n\n  5. You are not " +
				"required to accept this License, since you have not\nsigned it.  However, nothing" +
				" else grants you permission to modify or\ndistribute the Program or its derivativ" +
				"e works.  These actions are\nprohibited by law if you do not accept this License." +
				"  Therefore, by\nmodifying or distributing the Program (or any work based on the\n" +
				"Program), you indicate your acceptance of this License to do so, and\nall its ter" +
				"ms and conditions for copying, distributing or modifying\nthe Program or works ba" +
				"sed on it.\n\n  6. Each time you redistribute the Program (or any work based on th" +
				"e\nProgram), the recipient automatically receives a license from the\noriginal lic" +
				"ensor to copy, distribute or modify the Program subject to\nthese terms and condi" +
				"tions.  You may not impose any further\nrestrictions on the recipients\' exercise " +
				"of the rights granted herein.\nYou are not responsible for enforcing compliance b" +
				"y third parties to\nthis License.\n\n  7. If, as a consequence of a court judgment " +
				"or allegation of patent\ninfringement or for any other reason (not limited to pat" +
				"ent issues),\nconditions are imposed on you (whether by court order, agreement or" +
				"\notherwise) that contradict the conditions of this License, they do not\nexcuse y" +
				"ou from the conditions of this License.  If you cannot\ndistribute so as to satis" +
				"fy simultaneously your obligations under this\nLicense and any other pertinent ob" +
				"ligations, then as a consequence you\nmay not distribute the Program at all.  For" +
				" example, if a patent\nlicense would not permit royalty-free redistribution of th" +
				"e Program by\nall those who receive copies directly or indirectly through you, th" +
				"en\nthe only way you could satisfy both it and this License would be to\nrefrain e" +
				"ntirely from distribution of the Program.\n\nIf any portion of this section is hel" +
				"d invalid or unenforceable under\nany particular circumstance, the balance of the" +
				" section is intended to\napply and the section as a whole is intended to apply in" +
				" other\ncircumstances.\n\nIt is not the purpose of this section to induce you to in" +
				"fringe any\npatents or other property right claims or to contest validity of any\n" +
				"such claims; this section has the sole purpose of protecting the\nintegrity of th" +
				"e free software distribution system, which is\nimplemented by public license prac" +
				"tices.  Many people have made\ngenerous contributions to the wide range of softwa" +
				"re distributed\nthrough that system in reliance on consistent application of that" +
				"\nsystem; it is up to the author/donor to decide if he or she is willing\nto distr" +
				"ibute software through any other system and a licensee cannot\nimpose that choice" +
				".\n\nThis section is intended to make thoroughly clear what is believed to\nbe a co" +
				"nsequence of the rest of this License.\n\n  8. If the distribution and/or use of " +
				"the Program is restricted in\ncertain countries either by patents or by copyright" +
				"ed interfaces, the\noriginal copyright holder who places the Program under this L" +
				"icense\nmay add an explicit geographical distribution limitation excluding\nthose " +
				"countries, so that distribution is permitted only in or among\ncountries not thus" +
				" excluded.  In such case, this License incorporates\nthe limitation as if written" +
				" in the body of this License.\n\n  9. The Free Software Foundation may publish rev" +
				"ised and/or new versions\nof the General Public License from time to time.  Such " +
				"new versions will\nbe similar in spirit to the present version, but may differ in" +
				" detail to\naddress new problems or concerns.\n\nEach version is given a distinguis" +
				"hing version number.  If the Program\nspecifies a version number of this License " +
				"which applies to it and \"any\nlater version\", you have the option of following th" +
				"e terms and conditions\neither of that version or of any later version published " +
				"by the Free\nSoftware Foundation.  If the Program does not specify a version numb" +
				"er of\nthis License, you may choose any version ever published by the Free Softwa" +
				"re\nFoundation.\n\n  10. If you wish to incorporate parts of the Program into other" +
				" free\nprograms whose distribution conditions are different, write to the author\n" +
				"to ask for permission.  For software which is copyrighted by the Free\nSoftware F" +
				"oundation, write to the Free Software Foundation; we sometimes\nmake exceptions f" +
				"or this.  Our decision will be guided by the two goals\nof preserving the free st" +
				"atus of all derivatives of our free software and\nof promoting the sharing and re" +
				"use of software generally.\n\n\t\t\t    NO WARRANTY\n\n  11. BECAUSE THE PROGRAM IS LIC" +
				"ENSED FREE OF CHARGE, THERE IS NO WARRANTY\nFOR THE PROGRAM, TO THE EXTENT PERMIT" +
				"TED BY APPLICABLE LAW.  EXCEPT WHEN\nOTHERWISE STATED IN WRITING THE COPYRIGHT HO" +
				"LDERS AND/OR OTHER PARTIES\nPROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY OF ANY K" +
				"IND, EITHER EXPRESSED\nOR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WAR" +
				"RANTIES OF\nMERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.  THE ENTIRE RIS" +
				"K AS\nTO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.  SHOULD THE\nPROG" +
				"RAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING,\nREPAIR OR C" +
				"ORRECTION.\n\n  12. IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN " +
				"WRITING\nWILL ANY COPYRIGHT HOLDER, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR\nREDI" +
				"STRIBUTE THE PROGRAM AS PERMITTED ABOVE, BE LIABLE TO YOU FOR DAMAGES,\nINCLUDING" +
				" ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES ARISING\nOUT OF THE US" +
				"E OR INABILITY TO USE THE PROGRAM (INCLUDING BUT NOT LIMITED\nTO LOSS OF DATA OR " +
				"DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY\nYOU OR THIRD PARTIES OR A " +
				"FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER\nPROGRAMS), EVEN IF SUCH HOLDER " +
				"OR OTHER PARTY HAS BEEN ADVISED OF THE\nPOSSIBILITY OF SUCH DAMAGES.\n\n\t\t     END " +
				"OF TERMS AND CONDITIONS\n\n\t    How to Apply These Terms to Your New Programs\n\n  " +
				"If you develop a new program, and you want it to be of the greatest\npossible use" +
				" to the public, the best way to achieve this is to make it\nfree software which e" +
				"veryone can redistribute and change under these terms.\n\n  To do so, attach the f" +
				"ollowing notices to the program.  It is safest\nto attach them to the start of ea" +
				"ch source file to most effectively\nconvey the exclusion of warranty; and each fi" +
				"le should have at least\nthe \"copyright\" line and a pointer to where the full not" +
				"ice is found.\n\n    <one line to give the program\'s name and a brief idea of what" +
				" it does.>\n    Copyright (C) <year>  <name of author>\n\n    This program is free " +
				"software; you can redistribute it and/or modify\n    it under the terms of the GN" +
				"U General Public License as published by\n    the Free Software Foundation; eithe" +
				"r version 2 of the License, or\n    (at your option) any later version.\n\n    This" +
				" program is distributed in the hope that it will be useful,\n    but WITHOUT ANY " +
				"WARRANTY; without even the implied warranty of\n    MERCHANTABILITY or FITNESS FO" +
				"R A PARTICULAR PURPOSE.  See the\n    GNU General Public License for more details" +
				".\n\n    You should have received a copy of the GNU General Public License\n    alo" +
				"ng with this program; if not, write to the Free Software\n    Foundation, Inc., 5" +
				"9 Temple Place, Suite 330, Boston, MA  02111-1307  USA\n\n\nAlso add information on" +
				" how to contact you by electronic and paper mail.\n\nIf the program is interactive" +
				", make it output a short notice like this\nwhen it starts in an interactive mode:" +
				"\n\n    Gnomovision version 69, Copyright (C) year name of author\n    Gnomovision " +
				"comes with ABSOLUTELY NO WARRANTY; for details type `show w\'.\n    This is free s" +
				"oftware, and you are welcome to redistribute it\n    under certain conditions; ty" +
				"pe `show c\' for details.\n\nThe hypothetical commands `show w\' and `show c\' should" +
				" show the appropriate\nparts of the General Public License.  Of course, the comma" +
				"nds you use may\nbe called something other than `show w\' and `show c\'; they could" +
				" even be\nmouse-clicks or menu items--whatever suits your program.\n\nYou should al" +
				"so get your employer (if you work as a programmer) or your\nschool, if any, to si" +
				"gn a \"copyright disclaimer\" for the program, if\nnecessary.  Here is a sample; al" +
				"ter the names:\n\n  Yoyodyne, Inc., hereby disclaims all copyright interest in the" +
				" program\n  `Gnomovision\' (which makes passes at compilers) written by James Hack" +
				"er.\n\n  <signature of Ty Coon>, 1 April 1989\n  Ty Coon, President of Vice\n\nThis G" +
				"eneral Public License does not permit incorporating your program into\nproprietar" +
				"y programs.  If your program is a subroutine library, you may\nconsider it more u" +
				"seful to permit linking proprietary applications with the\nlibrary.  If this is w" +
				"hat you want to do, use the GNU Library General\nPublic License instead of this L" +
				"icense.\n";
			this.richTextBox1.WordWrap = false;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(291, 368);
			this.button1.Name = "button1";
			this.button1.TabIndex = 2;
			this.button1.Text = "OK";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 296);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(640, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Portions copyright 2002-2003 The Genghis Group (http://www.genghisgroup.com/).";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 328);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(640, 32);
			this.label3.TabIndex = 6;
			this.label3.Text = "In addition to the terms of the GPL above, permission is granted to link nprof ag" +
				"ainst the Magic Library and the Genghis Library.";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Location = new System.Drawing.Point(504, 8);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(144, 16);
			this.linkLabel1.TabIndex = 7;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://nprof.sourceforge.net";
			// 
			// AboutForm
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(656, 399);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.Text = "About nprof";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
