using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace kolpet.MazeSolver
{
    [TestClass]
    public class UnitTest1
    {
        #region XMLs
        private const string Level1Xml = @"<Maze>
   <Level>1</Level>
   <StartPoint>
       <Row>9</Row>
       <Column>1</Column>
   </StartPoint>
   <EscapePoint>
       <Row>7</Row>
       <Column>17</Column>
   </EscapePoint>
   <InsideItems>
       <Wall>
           <Row>2</Row>
           <Column>6</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>2</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>3</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>4</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>6</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>8</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>9</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>10</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>11</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>12</Column>
       </Wall>	  
	   <Wall>
           <Row>3</Row>
           <Column>13</Column>
       </Wall>	
	   <Wall>
           <Row>3</Row>
           <Column>14</Column>
       </Wall>	  
	   <Wall>
           <Row>3</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>4</Row>
           <Column>6</Column>
       </Wall>	 
	   <Wall>
           <Row>4</Row>
           <Column>15</Column>
       </Wall>	
	   <Wall>
           <Row>4</Row>
           <Column>16</Column>
       </Wall>	   
	   <Wall>
           <Row>5</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>4</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>6</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>8</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>9</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>10</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>11</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>12</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>6</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>6</Row>
           <Column>8</Column>
       </Wall>	 
	   <Wall>
           <Row>6</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>6</Row>
           <Column>14</Column>
       </Wall>	  
	   <Wall>
           <Row>6</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>7</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>7</Row>
           <Column>6</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>7</Row>
           <Column>8</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>11</Column>
       </Wall>	
	   <Wall>
           <Row>7</Row>
           <Column>13</Column>
       </Wall>	
	   <Wall>
           <Row>8</Row>
           <Column>3</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>8</Row>
           <Column>11</Column>
       </Wall>	  
	   <Wall>
           <Row>8</Row>
           <Column>13</Column>
       </Wall>	   
	   <Wall>
           <Row>8</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>16</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>3</Column>
       </Wall>	   
	   <Wall>
           <Row>9</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>9</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>9</Row>
           <Column>9</Column>
       </Wall>	  
	   <Wall>
           <Row>9</Row>
           <Column>11</Column>
       </Wall>	  
	   <Wall>
           <Row>9</Row>
           <Column>13</Column>
       </Wall>	
	   <Wall>
           <Row>10</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>9</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>11</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>13</Column>
       </Wall>	   
	   <Wall>
           <Row>10</Row>
           <Column>14</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>15</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>16</Column>
       </Wall>	   	   
	   <Wall>
           <Row>11</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>11</Row>
           <Column>7</Column>
       </Wall>	   
	   <Wall>
           <Row>11</Row>
           <Column>9</Column>
       </Wall>	  
	   <Wall>
           <Row>11</Row>
           <Column>11</Column>
       </Wall>
	   <Wall>
           <Row>11</Row>
           <Column>13</Column>
       </Wall>	   
	   <Wall>
           <Row>12</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>7</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>9</Column>
       </Wall>	
	   <Wall>
           <Row>12</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>13</Column>
       </Wall>	   
	   <Wall>
           <Row>13</Row>
           <Column>2</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>13</Row>
           <Column>4</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>13</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>13</Row>
           <Column>9</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>14</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>14</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>14</Row>
           <Column>9</Column>
       </Wall>	  
	   <Wall>
           <Row>14</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>4</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>6</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>15</Row>
           <Column>9</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>12</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>14</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>16</Column>
       </Wall>
	   <Wall>
           <Row>16</Row>
           <Column>9</Column>
       </Wall>
   </InsideItems>
</Maze>";
        private const string Level2Xml = @"<Maze>
   <Level>2</Level>
   <StartPoint>
       <Row>1</Row>
       <Column>9</Column>
   </StartPoint>
   <EscapePoint>
       <Row>17</Row>
       <Column>9</Column>
   </EscapePoint>
   <InsideItems>
       <Wall>
           <Row>2</Row>
           <Column>3</Column>
       </Wall>
	   <Wall>
           <Row>2</Row>
           <Column>12</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>3</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>5</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>6</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>7</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>8</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>9</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>10</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>12</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>14</Column>
       </Wall>	  
	   <Wall>
           <Row>3</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>4</Row>
           <Column>10</Column>
       </Wall>	 
	   <Wall>
           <Row>4</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>2</Column>
       </Wall>		   
	   <Wall>
           <Row>5</Row>
           <Column>3</Column>
       </Wall>	  	 
	   <Wall>
           <Row>5</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>6</Column>
       </Wall>	
	   <Wall>
           <Row>5</Row>
           <Column>7</Column>
       </Wall>		   
	   <Wall>
           <Row>5</Row>
           <Column>8</Column>
       </Wall>	    
	   <Wall>
           <Row>5</Row>
           <Column>10</Column>
       </Wall>	   
	   <Wall>
           <Row>5</Row>
           <Column>12</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>13</Column>
       </Wall>	 	   
	   <Wall>
           <Row>5</Row>
           <Column>15</Column>
       </Wall>
	   <Wall>
           <Row>6</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>6</Row>
           <Column>10</Column>
       </Wall>	 
	   <Wall>
           <Row>6</Row>
           <Column>13</Column>
       </Wall>	   
	   <Wall>
           <Row>6</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>2</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>7</Row>
           <Column>4</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>5</Column>
       </Wall>	  	 
	   <Wall>
           <Row>7</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>7</Row>
           <Column>8</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>9</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>10</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>11</Column>
       </Wall>		
	   <Wall>
           <Row>7</Row>
           <Column>12</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>13</Column>
       </Wall>	
	   <Wall>
           <Row>7</Row>
           <Column>15</Column>
       </Wall>	
	   <Wall>
           <Row>8</Row>
           <Column>7</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>15</Column>
       </Wall>	  	
	   <Wall>
           <Row>9</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>4</Column>
       </Wall>   
	   <Wall>
           <Row>9</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>6</Column>
       </Wall>  
	   <Wall>
           <Row>9</Row>
           <Column>7</Column>
       </Wall>	 	
	   <Wall>
           <Row>9</Row>
           <Column>9</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>10</Column>
       </Wall>  
	   <Wall>
           <Row>9</Row>
           <Column>11</Column>
       </Wall>	  
	   <Wall>
           <Row>9</Row>
           <Column>13</Column>
       </Wall>		
	   <Wall>
           <Row>9</Row>
           <Column>14</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>15</Column>
       </Wall>	
	   <Wall>
           <Row>10</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>13</Column>
       </Wall>	  
	   <Wall>
           <Row>11</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>11</Row>
           <Column>4</Column>
       </Wall>  
	   <Wall>
           <Row>11</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>11</Row>
           <Column>6</Column>
       </Wall>
	   <Wall>
           <Row>11</Row>
           <Column>7</Column>
       </Wall>		
	   <Trap>
           <Row>11</Row>
           <Column>8</Column>
       </Trap>   
	   <Wall>
           <Row>11</Row>
           <Column>9</Column>
       </Wall>		
	   <Wall>
           <Row>11</Row>
           <Column>10</Column>
       </Wall>  
	   <Wall>
           <Row>11</Row>
           <Column>11</Column>
       </Wall>	
	   <Wall>
           <Row>11</Row>
           <Column>12</Column>
       </Wall>
	   <Wall>
           <Row>11</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>11</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>11</Row>
           <Column>16</Column>
       </Wall>  
	   <Wall>
           <Row>12</Row>
           <Column>6</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>2</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>13</Row>
           <Column>4</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>6</Column>
       </Wall>	
	   <Wall>
           <Row>13</Row>
           <Column>8</Column>
       </Wall>	  
	   <Wall>
           <Row>13</Row>
           <Column>9</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>10</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>12</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>14</Column>
       </Wall>	 	
	   <Wall>
           <Row>13</Row>
           <Column>15</Column>
       </Wall>	
	   <Wall>
           <Row>14</Row>
           <Column>6</Column>
       </Wall>	  
	   <Wall>
           <Row>14</Row>
           <Column>10</Column>
       </Wall>	  
	   <Wall>
           <Row>14</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>4</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>6</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>15</Row>
           <Column>9</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>10</Column>
       </Wall>	   
	   <Wall>
           <Row>15</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>12</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>15</Column>
       </Wall>	 
	   <Trap>
           <Row>16</Row>
           <Column>13</Column>
       </Trap>
   </InsideItems>
</Maze>";
        private const string Level3Xml = @"<Maze>
   <Level>3</Level>
   <StartPoint>
       <Row>1</Row>
       <Column>9</Column>
   </StartPoint>
   <EscapePoint>
       <Row>17</Row>
       <Column>9</Column>
   </EscapePoint>
   <InsideItems>
       <Wall>
           <Row>2</Row>
           <Column>3</Column>
       </Wall>   
	   <Wall>
           <Row>3</Row>
           <Column>3</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>5</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>6</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>7</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>8</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>9</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>10</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>11</Column>
       </Wall>
	   <Wall>
           <Row>3</Row>
           <Column>12</Column>
       </Wall>	   
	   <Wall>
           <Row>3</Row>
           <Column>14</Column>
       </Wall>	  
	   <Wall>
           <Row>3</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>4</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>4</Row>
           <Column>12</Column>
       </Wall>		
	   <Wall>
           <Row>4</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>2</Column>
       </Wall>		   
	   <Wall>
           <Row>5</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>4</Column>
       </Wall>	  	 
	   <Wall>
           <Row>5</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>5</Row>
           <Column>7</Column>
       </Wall>	
	   <Wall>
           <Row>5</Row>
           <Column>9</Column>
       </Wall>		   
	   <Wall>
           <Row>5</Row>
           <Column>10</Column>
       </Wall>	    
	   <Wall>
           <Row>5</Row>
           <Column>11</Column>
       </Wall>	   
	   <Wall>
           <Row>5</Row>
           <Column>12</Column>
       </Wall>	  
	   <Wall>
           <Row>5</Row>
           <Column>13</Column>
       </Wall>	 	   
	   <Wall>
           <Row>5</Row>
           <Column>15</Column>
       </Wall>
	   <Wall>
           <Row>6</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>6</Row>
           <Column>7</Column>
       </Wall>	 
	   <Wall>
           <Row>6</Row>
           <Column>13</Column>
       </Wall>	   
	   <Wall>
           <Row>6</Row>
           <Column>15</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>3</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>7</Column>
       </Wall>	
	   <Wall>
           <Row>7</Row>
           <Column>8</Column>
       </Wall>
	   <Wall>
           <Row>7</Row>
           <Column>9</Column>
       </Wall>	  	 
	   <Wall>
           <Row>7</Row>
           <Column>10</Column>
       </Wall>	  
	   <Wall>
           <Row>7</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>7</Row>
           <Column>15</Column>
       </Wall>
	   <Wall>
           <Row>8</Row>
           <Column>5</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>7</Column>
       </Wall>	 
	   <Wall>
           <Row>8</Row>
           <Column>11</Column>
       </Wall>		
	   <Wall>
           <Row>8</Row>
           <Column>12</Column>
       </Wall>	  
	   <Wall>
           <Row>8</Row>
           <Column>13</Column>
       </Wall>	  
	   <Wall>
           <Row>8</Row>
           <Column>14</Column>
       </Wall>		
	   <Wall>
           <Row>8</Row>
           <Column>15</Column>
       </Wall>			
	   <Wall>
           <Row>9</Row>
           <Column>2</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>3</Column>
       </Wall>   
	   <Wall>
           <Row>9</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>7</Column>
       </Wall>  
	   <Wall>
           <Row>9</Row>
           <Column>8</Column>
       </Wall>	 	
	   <Wall>
           <Row>9</Row>
           <Column>9</Column>
       </Wall>	
	   <Wall>
           <Row>9</Row>
           <Column>11</Column>
       </Wall>
	   <Wall>
           <Row>10</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>9</Column>
       </Wall>	
	   <Wall>
           <Row>10</Row>
           <Column>11</Column>
       </Wall>		
	   <Wall>
           <Row>10</Row>
           <Column>13</Column>
       </Wall>	  
	   <Wall>
           <Row>10</Row>
           <Column>14</Column>
       </Wall>		
	   <Wall>
           <Row>10</Row>
           <Column>15</Column>
       </Wall>	
	   <Wall>
           <Row>11</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>11</Row>
           <Column>4</Column>
       </Wall>  
	   <Wall>
           <Row>11</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>11</Row>
           <Column>6</Column>
       </Wall>
	   <Wall>
           <Row>11</Row>
           <Column>7</Column>
       </Wall>		 
	   <Wall>
           <Row>11</Row>
           <Column>9</Column>
       </Wall>		
	   <Wall>
           <Row>11</Row>
           <Column>13</Column>
       </Wall>
	   <Wall>
           <Row>12</Row>
           <Column>7</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>9</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>10</Column>
       </Wall>	  
	   <Wall>
           <Row>12</Row>
           <Column>11</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>12</Column>
       </Wall>	
	   <Wall>
           <Row>12</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>14</Column>
       </Wall>	 
	   <Wall>
           <Row>12</Row>
           <Column>15</Column>
       </Wall>	
	   <Wall>
           <Row>12</Row>
           <Column>16</Column>
       </Wall>
	   <Wall>
           <Row>13</Row>
           <Column>2</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>3</Column>
       </Wall>	  
	   <Wall>
           <Row>13</Row>
           <Column>4</Column>
       </Wall>	 
	   <Wall>
           <Row>13</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>13</Row>
           <Column>7</Column>
       </Wall>	  	 
	   <Trap>
           <Row>13</Row>
           <Column>14</Column>
       </Trap>	 
	   <Wall>
           <Row>14</Row>
           <Column>5</Column>
       </Wall>	  
	   <Wall>
           <Row>14</Row>
           <Column>7</Column>
       </Wall>	  
	   <Wall>
           <Row>14</Row>
           <Column>8</Column>
       </Wall>		
	   <Wall>
           <Row>14</Row>
           <Column>9</Column>
       </Wall> 	 
	   <Wall>
           <Row>14</Row>
           <Column>10</Column>
       </Wall>	 
	   <Wall>
           <Row>14</Row>
           <Column>11</Column>
       </Wall>	
	   <Wall>
           <Row>14</Row>
           <Column>12</Column>
       </Wall>	 
	   <Wall>
           <Row>14</Row>
           <Column>13</Column>
       </Wall>	 
	   <Wall>
           <Row>14</Row>
           <Column>14</Column>
       </Wall>	
	   <Wall>
           <Row>14</Row>
           <Column>15</Column>
       </Wall>
	   <Wall>
           <Row>15</Row>
           <Column>3</Column>
       </Wall>	 
	   <Wall>
           <Row>15</Row>
           <Column>5</Column>
       </Wall>	
	   <Wall>
           <Row>15</Row>
           <Column>8</Column>
       </Wall>	 
	   <Wall>
           <Row>16</Row>
           <Column>3</Column>
       </Wall>	
	   <Wall>
           <Row>16</Row>
           <Column>8</Column>
       </Wall>	 
	   <Wall>
           <Row>16</Row>
           <Column>11</Column>
       </Wall>
   </InsideItems>
</Maze>";

        private string Level1Result = @"<Actions>
	<Step>
		<Direction>2</Direction>
		<CellNumber>1</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>3</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>3</Direction>
		<CellNumber>6</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>3</CellNumber>
	</Step>
	<Step>
		<Direction>3</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>7</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>1</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>1</CellNumber>
	</Step>
</Actions>
";
        private string Level2Result = @"<Actions>
	<Step>
		<Direction>4</Direction>
		<CellNumber>1</CellNumber>
	</Step>
	<Step>
		<Direction>1</Direction>
		<CellNumber>5</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>5</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>1</Direction>
		<CellNumber>3</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>1</Direction>
		<CellNumber>4</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>4</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>3</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>1</Direction>
		<CellNumber>3</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>7</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>1</CellNumber>
	</Step>
</Actions>
";
        private string Level3Result = @"<Actions>
	<Step>
		<Direction>4</Direction>
		<CellNumber>1</CellNumber>
	</Step>
	<Step>
		<Direction>2</Direction>
		<CellNumber>7</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>7</CellNumber>
	</Step>
	<Step>
		<Direction>1</Direction>
		<CellNumber>4</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
	<Rotate>
		<District>9</District>
		<Direction>2</Direction>
	</Rotate>
	<Step>
		<Direction>4</Direction>
		<CellNumber>4</CellNumber>
	</Step>
	<Step>
		<Direction>1</Direction>
		<CellNumber>3</CellNumber>
	</Step>
	<Step>
		<Direction>4</Direction>
		<CellNumber>2</CellNumber>
	</Step>
</Actions>
";
        #endregion

        [TestMethod]
        public void TestLevel1()
        {
            string result = RunTest(Level1Xml);
            EvaluateResult(Level1Result, result);
        }

        [TestMethod]
        public void TestLevel2()
        {
            string result = RunTest(Level2Xml);
            EvaluateResult(Level2Result, result);
        }

        [TestMethod]
        public void TestLevel3()
        {
            string result = RunTest(Level3Xml);
            EvaluateResult(Level3Result, result);
        }

        [TestMethod]
        public void TestCustomLevel()
        {
            TestMaze testMaze = new TestMaze(3, new Point(2, 1), new Point(2, 17))
                .DrawWalls(2, 3, 15, 3)
                .DrawWalls(16, 5, 3, 5)
                .DrawWalls(2, 7, 15, 7)
                .DrawWalls(16, 9, 3, 9)
                .DrawWalls(2, 11, 15, 11)
                .DrawWalls(16, 13, 3, 13)
                .DrawWalls(2, 15, 15, 15)
                .AddTrap(8, 8);

            string result = RunTest(testMaze.Print());
            EvaluateResult(string.Empty, result);
        }

        [TestMethod]
        public void TestExtensions()
        {
            // Rotate
            {
                Direction dir = Direction.Up;

                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Right, dir);
                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Down, dir);
                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Left, dir);
                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Up, dir);

                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Left, dir);
                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Down, dir);
                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Right, dir);
                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Up, dir);

                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Down, dir);
                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Up, dir);

                dir = Direction.Right;

                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Left, dir);
                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Right, dir);
            }

            // GetDistrict
            {
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(0, 0));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(0, 5));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(0, 16));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(5, 0));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(5, 16));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(16, 0));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(16, 5));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(16, 16));

                Assert.AreEqual(District.TopLeft, Extensions.GetDistrict(1, 1));
                Assert.AreEqual(District.TopLeft, Extensions.GetDistrict(5, 5));

                Assert.AreEqual(District.Top, Extensions.GetDistrict(1, 6));
                Assert.AreEqual(District.Top, Extensions.GetDistrict(5, 10));

                Assert.AreEqual(District.TopRight, Extensions.GetDistrict(1, 11));
                Assert.AreEqual(District.TopRight, Extensions.GetDistrict(5, 15));

                Assert.AreEqual(District.Left, Extensions.GetDistrict(6, 1));
                Assert.AreEqual(District.Left, Extensions.GetDistrict(10, 5));

                Assert.AreEqual(District.Middle, Extensions.GetDistrict(6, 6));
                Assert.AreEqual(District.Middle, Extensions.GetDistrict(10, 10));

                Assert.AreEqual(District.Right, Extensions.GetDistrict(6, 11));
                Assert.AreEqual(District.Right, Extensions.GetDistrict(10, 15));

                Assert.AreEqual(District.BottomLeft, Extensions.GetDistrict(11, 1));
                Assert.AreEqual(District.BottomLeft, Extensions.GetDistrict(15, 5));

                Assert.AreEqual(District.Bottom, Extensions.GetDistrict(11, 6));
                Assert.AreEqual(District.Bottom, Extensions.GetDistrict(15, 10));

                Assert.AreEqual(District.BottomRight, Extensions.GetDistrict(11, 11));
                Assert.AreEqual(District.BottomRight, Extensions.GetDistrict(15, 15));
            }

            // AdjustPoint
            {
                /* .1...  District.TopLeft, CodeRotation.Right
                 * ....4  1: 1,2
                 * .....  2: 4,1
                 * 2....  3: 5,4
                 * ...3.  4: 2,5
                 */
                int row = 1; int column = 2;
                Extensions.AdjustPoint(CodeRotation.Up, ref row, ref column);
                Assert.AreEqual(1, row);
                Assert.AreEqual(2, column);

                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(4, row);
                Assert.AreEqual(1, column);
                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(5, row);
                Assert.AreEqual(4, column);
                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(2, row);
                Assert.AreEqual(5, column);
                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(1, row);
                Assert.AreEqual(2, column);

                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(2, row);
                Assert.AreEqual(5, column);
                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(5, row);
                Assert.AreEqual(4, column);
                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(4, row);
                Assert.AreEqual(1, column);
                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(1, row);
                Assert.AreEqual(2, column);
            }

            // PreviewRotation
            {
                /* .X...  District.TopLeft
                 * HX...
                 * .X...
                 * .X...
                 * .X...
                 */
                TestMaze testMaze = new TestMaze(3, new Point(2, 1), new Point(2, 17))
                    .DrawWalls(2, 3, 6, 3)
                    .AddTrap(3, 2);

                Maze maze = new Maze(testMaze.Print());

                Assert.AreEqual(Tile.Empty, maze.PreviewRotation(CodeRotation.Right, 1, 1));
                Assert.AreEqual(Tile.Trap, maze.PreviewRotation(CodeRotation.Right, 1, 4));
                for (int i = 1; i <= 5; i++)
                {
                    Assert.AreEqual(Tile.Wall, maze.PreviewRotation(CodeRotation.Right, 2, i));
                }

                Assert.AreEqual(Tile.Empty, maze[1, 1]);
                Assert.AreEqual(Tile.Trap, maze[2, 1]);
                for (int i = 1; i <= 5; i++)
                {
                    Assert.AreEqual(Tile.Wall, maze[i, 2]);
                }
            }
        }

        private string RunTest(string xml)
        {
            Task<string> solver = Task.Run(() =>
            {
                Maze maze = new Maze(xml);
                Agency agency = new Agency(maze);
                Agent best = agency.Solve();
                return best.Result();
            });

            Task timeout = Task.Delay(60 * 1000);

            Task.WaitAny(solver, timeout);

            Assert.IsFalse(solver.IsFaulted, "test threw exception, ex: " + solver.Exception?.InnerException);
            Assert.IsTrue(solver.IsCompleted, "test timeouted after 60s");

            return solver.Result;
        }

        private void EvaluateResult(string expectedResult, string actualResult)
        {
            int actualSteps = CountSteps(actualResult);
            if (!string.IsNullOrEmpty(expectedResult))
            {
                if (actualResult != expectedResult)
                {
                    Logger.LogMessage("result deviates from expected result");
                }
                int expectedSteps = CountSteps(expectedResult);
                Logger.LogMessage($"expected steps: {expectedSteps}; actual steps: {actualSteps}");
                Logger.LogMessage(actualResult);

                Assert.IsTrue(actualSteps <= expectedSteps, "test performed worse than previous best");
                Assert.AreEqual(expectedSteps, actualSteps, "test performed better than previous best, adjust test");
            }
            else
            {
                Logger.LogMessage($"steps: {actualSteps}");
                Logger.LogMessage(actualResult);
            }
        }

        private int CountSteps(string result) 
        {
            MatchCollection matches = Regex.Matches(result, "<Step>\\s+\\S+\\s+\\D+(\\d+)");
            int steps = 0;
            foreach (Match match in matches)
            {
                steps += int.Parse(match.Groups[1].Value);
            }

            matches = Regex.Matches(result, "<Rotate>");
            steps += matches.Count * 5;
            return steps;
        }

        private class TestMaze
        {
            public int Level { get; set; }

            public Point Start { get; private set; } = new Point(0, 0);

            public Point End { get; private set; } = new Point(0, 0);

            public List<Point> Walls { get; private set; } = new List<Point>();

            public List<Point> Traps { get; private set; } = new List<Point>();

            public TestMaze(int level, Point start, Point end)
            {
                Level = level;
                Start = start;
                End = end;
            }

            public TestMaze AddTrap(int x, int y)
            {
                Traps.Add(new Point(x, y));
                return this;
            }

            public TestMaze DrawWalls(int x1, int y1, int x2, int y2)
            {
                if (x1 != x2 && y1 != y2) return this;
                int dx = Math.Sign(x2 - x1);
                int dy = Math.Sign(y2 - y1);
                int i = x1; int j = y1;
                while (i != x2 || j != y2)
                {
                    Point p = new Point(i, j);
                    if (!Walls.Contains(p))
                    {
                        Walls.Add(p);
                    }
                    i += dx; j += dy;
                }
                Point p2 = new Point(x2, y2);
                if (!Walls.Contains(p2))
                {
                    Walls.Add(p2);
                }
                return this;
            }

            public string Print()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine( "<Maze>");
                sb.AppendLine($"\t<Level>{Level}</Level>");
                sb.AppendLine( "\t<StartPoint>");
                sb.AppendLine($"\t\t<Row>{Start.x}</Row>");
                sb.AppendLine($"\t\t<Column>{Start.y}</Column>");
                sb.AppendLine( "\t</StartPoint>");
                sb.AppendLine( "\t<EscapePoint>");
                sb.AppendLine($"\t\t<Row>{End.x}</Row>");
                sb.AppendLine($"\t\t<Column>{End.y}</Column>");
                sb.AppendLine( "\t</EscapePoint>");
                sb.AppendLine( "\t<InsideItems>");
                foreach(Point point in Walls)
                {
                    sb.AppendLine("\t\t<Wall>");
                    sb.AppendLine($"\t\t\t<Row>{point.x}</Row>");
                    sb.AppendLine($"\t\t\t<Column>{point.y}</Column>");
                    sb.AppendLine("\t\t</Wall>");
                }
                foreach (Point point in Traps)
                {
                    sb.AppendLine("\t\t<Trap>");
                    sb.AppendLine($"\t\t\t<Row>{point.x}</Row>");
                    sb.AppendLine($"\t\t\t<Column>{point.y}</Column>");
                    sb.AppendLine("\t\t</Trap>");
                }
                sb.AppendLine( "\t</InsideItems>");
                sb.AppendLine( "</Maze>");
                return sb.ToString();
            }
        }
    }
}