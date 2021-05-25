/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;*/
using System;
using System.Threading;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;


namespace WebApp.myDetectorServer
{
	public class Line
	{
		public float a, b;
		public Line(float a, float b)
		{
			this.a = a;
			this.b = b;
		}
		public Line()
		{
			this.a = 0;
			this.b = 0;
		}
		public float f(float x)
		{
			return this.a * x + this.b;
		}
	}

	public class Point
	{
		public float x, y;
		public Point(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}

	public class Circle
	{

		public Point center;
		public float radius;
		public Circle(Point c, float r)
		{
			this.center = c;
			this.radius = r;
		}
		public Circle()
		{
			this.center = new Point(0, 0);
			this.radius = 0;
		}

		public float distance(Point p)
		{
			double dx = center.x - p.x;
			double dy = center.y - p.y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}
	}

	public class DetectionUtil
	{
		public float Avg(float[] x, int size)
		{
			float AVG = 0; // expected value
						   // calulating EV
			for (int i = 0; i < size; i++)
			{
				AVG += x[i];
			}
			AVG /= size;
			return AVG;
		}

		// returns the variance of X and Y
		public float Var(float[] x, int size)
		{
			float EV = Avg(x, size);
			float temp = 0;
			float Var = 0; // variance
						   // calculating var
			for (int i = 0; i < size; i++)
			{
				temp = x[i] - EV;
				Var += temp * temp;
			}
			Var /= size;

			return Var;
		}

		// returns the covariance of X and Y
		public float Cov(float[] x, float[] y, int size)
		{
			float Ex = Avg(x, size);
			float Ey = Avg(y, size);
			float cov = 0;
			for (int i = 0; i < size; i++)
			{
				cov += (x[i] - Ex) * (y[i] - Ey);
			}
			cov /= size;
			return cov;
		}


		// returns the Pearson correlation coefficient of X and Y
		public float Pearson(float[] x, float[] y, int size)
		{
			double Ex = Var(x, size);
			double Ey = Var(y, size);
			double cov = Cov(x, y, size);
			double p = cov / Math.Sqrt(Ex * Ey);
			return (float)p;
		}

		// performs a linear regression and returns the line equation
		public Line Linear_reg(Point[] points, int size)
		{
			float[] x = new float[size];
			float[] y = new float[size];
			for (int i = 0; i < size; i++)
			{
				x[i] = points[i].x;
				y[i] = points[i].y;
			}
			float Ex = Avg(x, size);
			float Ey = Avg(y, size);
			float a = Cov(x, y, size) / Var(x, size);
			float b = Ey - (a * Ex);
			//line* l = new Line(a,b)
			Line l = new Line(a, b);
			return l;
		}

		// returns the deviation between point p and the line equation of the points
		float Dev(Point p, Point[] points, int size)
		{
			Line l = Linear_reg(points, size);
			return Dev(p, l);
		}

		// returns the deviation between point p and the line
		float Dev(Point p, Line l)
		{
			float f = p.y - l.f(p.x);
			return Math.Abs(f);
		}


	}

	public class AnomalyReport
	{
		public string description { set; get; }
		public long timeStep { set; get; }
		public AnomalyReport(string description, long timeStep)
		{
			this.description = description;
			this.timeStep = timeStep;
		}


	}

	public abstract class TimeSeriesAnomalyDetector
	{
		public abstract void LearnNormal(in TimeSeries ts);
		public abstract List<AnomalyReport> Detect(in TimeSeries ts);
	}

	public class TimeSeries
	{

		private List<string> titles;
		private Dictionary<float, List<float>> db;

		public void print()
		{
			Console.Write("time");
			for (int i = 0; i < this.titles.Count; i++)
			{
				Console.Write("," + this.titles[i]);
			}
			Console.WriteLine();
			int count = 0;
			foreach (KeyValuePair<float, List<float>> pair in this.db)
			{
				Console.WriteLine(pair.Key);
				count++;
				for (int i = 0; i < pair.Value.Count; i++)
				{
					Console.Write("," + this.titles[i]);
				}
				Console.WriteLine();
			}
		}
		public List<float> get_time_vector()
		{
			List<float> timevector = new List<float>();
			foreach (KeyValuePair<float, List<float>> pair in this.db)
			{
				timevector.Add(pair.Key);
			}
			return timevector;
		}
		public List<List<float>> get_data_by_categories()
		{
			List<List<float>> v = new List<List<float>>();
			foreach (string str in this.titles)
			{
				v.Add(new List<float>());
			}
			foreach (KeyValuePair<float, List<float>> p in this.db)
			{
				for (int i = 0; i < p.Value.Count; i++)
				{
					v[i].Add(p.Value[i]);
				}
			}
			return v;

			/*List<List<float>> arr = new List<List<float>>();
			foreach (KeyValuePair<float, List<float>> pair in this.db)
			{
				arr.Add(new List<float>(pair.Value));
			}
			return arr;*/

		}
		public string title_by_number(int num)
		{
			if (num >= 0 && num < this.titles.Count)
			{
				return this.titles[num];
			}
			return "NULL";
		}
		public int number_by_title(string str)
		{
			for (int i = 0; i < this.titles.Count; i++)
			{
				if (this.titles[i].Equals(str))
				{
					return i;
				}
			}
			return -1;
		}
		public List<float> data_by_time(float timestep)
		{
			return new List<float>(this.db[timestep]);
		}

		public TimeSeries(in string CSVfileName)
		{
			System.IO.StreamReader myfile = new System.IO.StreamReader(CSVfileName);
			string line;
			line = myfile.ReadLine();
			this.db = new Dictionary<float, List<float>>();

			this.titles = new List<string>(line.Split(','));
			float t = 0;
			string[] arr;
			while ((line = myfile.ReadLine()) != null)
			{
				arr = line.Split(',');
				t++;
				float[] f = new float[arr.Length];
				for (int i = 0; i < arr.Length; i++)
				{
					f[i] = float.Parse(arr[i]);
				}
				this.db.Add(t, new List<float>(f));
			}
		}

	}

	public class CorrelatedFeatures
	{
		public string feature1, feature2;  // names of the correlated features
		public float corrlation;
		public Line lin_reg;
		public Circle circle;
		public float threshold;

		public CorrelatedFeatures()
		{

		}
	}

	public class SimpleAnomalyDetector : TimeSeriesAnomalyDetector
	{
		private List<CorrelatedFeatures> cf;
		private float threshhold;

		public SimpleAnomalyDetector()
		{
			this.cf = new List<CorrelatedFeatures>();
			this.threshhold = 0.9F;
		}

		public override void LearnNormal(in TimeSeries ts)
		{
			List<CorrelatedFeatures> corelations = this.FindCorelations(ts);
			this.CheckCorrelations(ts, corelations);
			return;
		}

		public override List<AnomalyReport> Detect(in TimeSeries ts)
		{
			List<AnomalyReport> v = new List<AnomalyReport>();
			List<float> timevector = ts.get_time_vector();
			int x, y;
			List<float> values;
			foreach (float t in timevector)
			{
				values = ts.data_by_time(t);
				foreach (CorrelatedFeatures cor in this.cf)
				{
					x = ts.number_by_title(cor.feature1);
					y = ts.number_by_title(cor.feature2);
					if (Math.Abs(values[y] - cor.lin_reg.f(values[x])) > cor.threshold)
					{
						v.Add(new AnomalyReport(cor.feature1 + "-" + cor.feature2, (long)t));
					}
				}
			}
			return v;
		}
		public List<CorrelatedFeatures> FindCorelations(TimeSeries ts)
		{
			DetectionUtil util = new DetectionUtil();
			List<List<float>> v = ts.get_data_by_categories();
			List<CorrelatedFeatures> corelations = new List<CorrelatedFeatures>();
			List<float> x;
			List<float> y;
			CorrelatedFeatures feature;
			float max = 0;
			int index;
			float corelation = 0;
			for (int i = 0; i < v.Count; i++)
			{
				max = 0;
				index = i;
				for (int j = i + 1; j < v.Count; j++)
				{
					x = v[i];
					y = v[j];
					corelation = util.Pearson(x.ToArray(), y.ToArray(), x.Count);
					if (Math.Abs(max) < Math.Abs(corelation))
					{
						max = corelation;
						index = j;
					}
				}
				feature = new CorrelatedFeatures();
				feature.corrlation = max;
				feature.feature1 = ts.title_by_number(i);
				feature.feature2 = ts.title_by_number(index);
				if (max > 0)
				{
					corelations.Add(feature);
				}

			}
			return corelations;
		}

		public void CheckCorrelations(in TimeSeries ts, List<CorrelatedFeatures> corelations)
		{
			DetectionUtil util = new DetectionUtil();
			int i, j;
			float threshhold;
			List<List<float>> v = ts.get_data_by_categories();
			List<Point> points;
			List<float> x;
			List<float> y;
			foreach (CorrelatedFeatures cor in corelations)
			{
				i = ts.number_by_title(cor.feature1);
				j = ts.number_by_title(cor.feature2);
				x = v[i];
				y = v[j];
				if (Math.Abs(cor.corrlation) >= this.threshhold)
				{
					points = new List<Point>();
					for (int k = 0; k < x.Count; k++)
					{
						points.Add(new Point(x[k], y[k]));
					}
					cor.lin_reg = util.Linear_reg(points.ToArray(), x.Count);

					threshhold = 0;
					for (int k = 0; k < x.Count; k++)
					{
						threshhold = Math.Max(Math.Abs(y[k] - cor.lin_reg.f(x[k])), threshhold);
					}
					cor.threshold = (float)(threshhold * 1.1);
					this.cf.Add(cor);
				}

			}
		}
		public List<CorrelatedFeatures> GetNormalModel()
		{
			return new List<CorrelatedFeatures>(this.cf);
		}
		public void ChangeThreshhold(float threshhold)
		{
			this.threshhold = threshhold;
		}
		public float GetTreshhold()
		{
			return this.threshhold;
		}
	}

	public class MinCircleUtil
	{
		public float Distance(Point p1, Point p2)
		{
			double dx = p1.x - p2.x;
			double dy = p1.y - p2.y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public Circle Trivial_circle(List<Point> R)
		{
			float x, y;
			double r, dx, dy;
			switch (R.Count)
			{
				case 0:
					return new Circle(new Point(0, 0), 0);
				case 1:
					return new Circle(R[0], 0);
				case 2:
					x = (R[0].x + R[1].x) / 2;
					y = (R[0].y + R[1].y) / 2;
					dx = R[0].x - R[1].x;
					dy = R[0].y - R[1].y;
					r = Math.Sqrt(dx * dx + dy * dy) / 2;
					return new Circle(new Point(x, y), (float)r);
				default:
					float x1 = R[0].x, y1 = R[0].y, x2 = R[1].x, y2 = R[1].y, x3 = R[2].x, y3 = R[2].y;
					float b = (x1 * x1 + y1 * y1 - x2 * x2 - y2 * y2) * (x3 - x1);
					b -= (x1 * x1 + y1 * y1 - x3 * x3 - y3 * y3) * (x2 - x1);
					b /= (y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1);
					float a = x1 * x1 + y1 * y1 - x3 * x3 - y3 * y3 + (y1 - y3) * b;
					a /= x3 - x1;
					float c = (-1) * (x1 * x1 + y1 * y1 + x1 * a + y1 * b);
					x = a / (-2);
					y = b / (-2);
					r = Math.Sqrt(x * x + y * y - c);
					return new Circle(new Point(x, y), (float)r);
			}

		}



		public bool Is_in_circle(Point point, Circle circle)
		{
			if (circle.distance(point) > circle.radius)
			{
				return false;
			}
			return true;
		}



		public Circle FindMinCircle(List<Point> points, List<Point> R)
		{
			if (points.Count == 0 || R.Count == 3)
			{
				return Trivial_circle(R);
			}
			Point p = points[points.Count - 1];
			points.RemoveAt(points.Count - 1);
			Circle circle = FindMinCircle(points, R);
			points.Add(p);
			double dx = (p.x - circle.center.x);
			double dy = (p.y - circle.center.y);
			double d = Math.Sqrt(dx * dx + dy * dy);
			if (d <= circle.radius)
			{
				return circle;
			}
			R.Add(p);
			points.RemoveAt(points.Count - 1);
			circle = FindMinCircle(points, R);
			R.RemoveAt(R.Count - 1);
			points.Add(p);
			return circle;
		}

		public Circle FindMinCircle(Point[] points, int size)
		{
			List<Point> v = new List<Point>(points);
			return FindMinCircle(v, new List<Point>());
		}
	}

	public class HybridAnomalyDetector : SimpleAnomalyDetector
	{
		private List<CorrelatedFeatures> ccf;
		public HybridAnomalyDetector() : base()
		{
			this.ccf = new List<CorrelatedFeatures>();
		}

		public override void LearnNormal(in TimeSeries ts)
		{
			List<CorrelatedFeatures> corelations = FindCorelations(ts);
			base.CheckCorrelations(ts, corelations);
			CheckCorrelations(ts, corelations);
			return;
		}
		public override List<AnomalyReport> Detect(in TimeSeries ts)
		{
			List<AnomalyReport> v = base.Detect(ts);
			List<float> timevector = ts.get_time_vector();
			int x, y;
			List<float> values = new List<float>();
			foreach (float t in timevector)
			{
				values = ts.data_by_time(t);
				foreach (CorrelatedFeatures cor in this.ccf)
				{
					x = ts.number_by_title(cor.feature1);
					y = ts.number_by_title(cor.feature2);
					if (cor.circle.distance(new Point(values[x], values[y])) > cor.threshold)
					{
						v.Add(new AnomalyReport(cor.feature1 + "-" + cor.feature2, (long)t));
					}
				}
			}
			/*for (auto anomaly : v) {
				cout << anomaly.description << endl;
			}*/


			return v;
		}
		public new List<CorrelatedFeatures> GetNormalModel()
		{
			List<CorrelatedFeatures> lst = base.GetNormalModel();
			lst.AddRange(new List<CorrelatedFeatures>(this.ccf));
			return lst;
		}
		public new void CheckCorrelations(in TimeSeries ts, List<CorrelatedFeatures> corelations)
		{
			int i, j;
			List<List<float>> v = ts.get_data_by_categories();
			List<Point> points;
			List<Point> r = new List<Point>();
			List<float> x;
			List<float> y;
			MinCircleUtil minCircleUtil = new MinCircleUtil();
			foreach (CorrelatedFeatures cor in corelations)
			{
				i = ts.number_by_title(cor.feature1);
				j = ts.number_by_title(cor.feature2);
				x = v[i];
				y = v[j];
				if (Math.Abs(cor.corrlation) < this.GetTreshhold() && Math.Abs(cor.corrlation) >= 0.5)
				{
					points = new List<Point>();
					for (int k = 0; k < x.Count; k++)
					{
						points.Add(new Point(x[k], y[k]));
					}
					r = new List<Point>();
					cor.circle = minCircleUtil.FindMinCircle(points, r);
					cor.threshold = cor.circle.radius * (float)1.1;
					this.ccf.Add(cor);
				}

			}
		}
	}

	public class DetectorServer
	{
		private TimeSeriesAnomalyDetector detector;
		private TimeSeries train;
        private TimeSeries test;
        private string pathToSave;
	
		private List<AnomalyReport> reports;

		public DetectorServer(string csvTrain, string csvTest, string type,string path)
		{
			this.train = new TimeSeries(csvTrain);
            this.test = new TimeSeries(csvTest);
			this.pathToSave = path;
			if (type.Equals("regression"))
			{
				this.detector = new SimpleAnomalyDetector();
			}
			else
			{
				this.detector = new HybridAnomalyDetector();
			}
			this.detector.LearnNormal(train);
			this.reports = detector.Detect(test);
		}

		public void Serialize()
		{
			string json = JsonSerializer.Serialize(this.reports);
			string path =this.pathToSave;
			Thread.Sleep(2000);
			File.WriteAllText(path, json);


		}

		public void PrintReports()
		{
			foreach (AnomalyReport report in reports)
			{
				Console.WriteLine(report.timeStep + " " + report.description);
			}
		}
	}


	public class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 4)
			{
				DetectorServer server = new DetectorServer(args[0], args[1], args[2], args[3]);
				server.Serialize();
				Console.WriteLine("done");
			}
		}
	}

}